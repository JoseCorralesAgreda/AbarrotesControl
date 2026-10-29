"use strict";
const $ = selector => document.querySelector(selector);
const state = { products: [], categories: [], providers: [], stock: [], alerts: [] };
const titles = { inventario: "Inventario", producto: "Nuevo producto", entrada: "Registrar entrada", consulta: "Consultar entrada", venta: "Registrar venta", stock: "Stock y alertas" };
const number = new Intl.NumberFormat("es-BO", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const escapeHtml = value => String(value ?? "").replace(/[&<>"']/g, char => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[char]));
const today = () => {
    const now = new Date();
    return [now.getFullYear(), String(now.getMonth() + 1).padStart(2, "0"), String(now.getDate()).padStart(2, "0")].join("-");
};
function notice(message, error = false) {
    $("#notice").textContent = message;
    $("#notice").classList.toggle("error", error);
    $("#notice").hidden = !message;
}
function showView(name) {
    if (!titles[name]) return;
    document.querySelectorAll(".view").forEach(view => { view.hidden = view.id !== name; });
    document.querySelectorAll(".nav").forEach(button => button.classList.toggle("active", button.dataset.view === name));
    $("#page-title").textContent = titles[name];
    notice("");
    if (name === "stock") refreshStock().catch(error => { if (!$("#stock").hidden) notice(error.message, true); });
    if (name === "venta") refresh().catch(error => { if (!$("#venta").hidden) notice(error.message, true); });
}
async function api(path, body) {
    let response;
    try {
        response = await fetch("/api/" + path, {
            method: body === undefined ? "GET" : "POST",
            headers: body === undefined ? { Accept: "application/json" } : { Accept: "application/json", "Content-Type": "application/json" },
            ...(body === undefined ? {} : { body: JSON.stringify(body) })
        });
    } catch {
        throw new Error("No se pudo conectar con la API. Comprueba que la aplicación esté ejecutándose.");
    }
    const payload = await response.json().catch(() => ({}));
    if (!response.ok) {
        const details = payload.errors;
        const errors = Array.isArray(details) ? details.map(item => item.motivo !== undefined
            ? (item.producto || "Producto #" + item.productoId) + ": " + item.motivo +
                " · Disponible: " + item.disponible + " · Solicitado: " + item.solicitado
            : item.field + ": " + item.error) : [];
        if (details?.proveedorIds?.length) errors.push("Proveedores: " + details.proveedorIds.join(", "));
        if (details?.productoIds?.length) errors.push("Productos: " + details.productoIds.join(", "));
        throw new Error([payload.message || "No se pudo completar la operación (" + response.status + ").", ...errors].join("\n"));
    }
    return payload.data;
}
function fillSelect(select, items, placeholder) {
    const selected = select.value;
    select.replaceChildren(new Option(placeholder, ""));
    items.forEach(item => select.add(new Option(item.nombre, item.id)));
    select.value = selected;
}
function renderProducts() {
    const search = $("#search").value.trim().toLocaleLowerCase("es");
    const category = $("#filter-category").value;
    const low = $("#filter-low").checked;
    const products = state.products.filter(p =>
        p.nombre.toLocaleLowerCase("es").includes(search) &&
        (!category || p.categoriaId === Number(category)) &&
        (!low || (p.activo && p.stockActual <= p.stockMinimo)));
    $("#count").textContent = state.products.length;
    $("#units").textContent = state.products.filter(p => p.activo).reduce((total, p) => total + p.stockActual, 0);
    $("#low").textContent = state.products.filter(p => p.activo && p.stockActual <= p.stockMinimo).length;
    $("#products").innerHTML = products.map(p =>
        '<tr><td>' + escapeHtml(p.nombre) + '</td><td>' + escapeHtml(p.categoriaNombre) +
        '</td><td>' + number.format(p.precioVenta) + '</td><td>' + p.stockActual + ' / ' + p.stockMinimo +
        '</td><td>' + escapeHtml(p.fechaVencimiento || "Sin vencimiento") + '</td><td><span class="badge ' +
        (!p.activo ? "off" : p.stockActual <= p.stockMinimo ? "low" : "") + '">' +
        (!p.activo ? "Inactivo" : p.stockActual <= p.stockMinimo ? "Stock bajo" : "Disponible") + '</span></td></tr>'
    ).join("") || '<tr><td colspan="6">No hay productos para mostrar.</td></tr>';
}
async function refresh() {
    $("#refresh").disabled = true;
    try {
        const [products, categories, providers] = await Promise.all([api("productos"), api("categorias"), api("proveedores")]);
        Object.assign(state, { products, categories, providers });
        fillSelect($("#product-category"), categories, "Selecciona una categoría");
        fillSelect($("#filter-category"), categories, "Todas las categorías");
        fillSelect($("#entry-provider"), providers, "Selecciona un proveedor");
        document.querySelectorAll(".detail-product").forEach(select => fillSelect(select, products.filter(p => p.activo), "Selecciona un producto"));
        renderProducts();
        updateSale();
    } finally {
        $("#refresh").disabled = false;
    }
}
function addDetail() {
    const row = document.createElement("div");
    row.className = "detail-row";
    row.innerHTML = '<label>Producto<select class="detail-product" required></select></label>' +
        '<label>Cantidad<input class="detail-quantity" type="number" min="1" max="2147483647" step="1" required value="1"></label>' +
        '<label>Costo unitario<input class="detail-cost" type="number" min="0.01" max="99999999.99" step="0.01" required></label>' +
        '<button type="button" class="secondary remove-detail">Quitar</button>';
    fillSelect(row.querySelector("select"), state.products.filter(p => p.activo), "Selecciona un producto");
    row.querySelector("button").addEventListener("click", () => {
        if ($("#details").children.length === 1) return notice("La entrada debe tener al menos un producto.", true);
        row.remove();
        updateTotal();
    });
    row.addEventListener("input", updateTotal);
    $("#details").append(row);
}
function details() {
    return [...document.querySelectorAll(".detail-row")].map(row => ({
        productoId: Number(row.querySelector(".detail-product").value),
        cantidad: Number(row.querySelector(".detail-quantity").value),
        costoUnitario: Number(row.querySelector(".detail-cost").value)
    }));
}
function updateTotal() {
    $("#entry-total").textContent = number.format(details().reduce((sum, d) => sum + d.cantidad * d.costoUnitario, 0));
}
function renderEntry(entry) {
    const result = $("#entry-result");
    result.hidden = false;
    result.innerHTML = '<h2>Entrada #' + entry.id + '</h2><p>Proveedor: ' + escapeHtml(entry.proveedorNombre) +
        ' · Fecha: ' + escapeHtml(entry.fechaEntrada) + ' · Documento: ' + escapeHtml(entry.nroDocumento || "Sin documento") +
        '</p><div class="table-wrap"><table><thead><tr><th>Producto</th><th>Cantidad</th><th>Costo</th><th>Subtotal</th><th>Stock actual</th></tr></thead><tbody>' +
        entry.detalles.map(d => '<tr><td>' + escapeHtml(d.productoNombre) + '</td><td>' + d.cantidad +
            '</td><td>' + number.format(d.costoUnitario) + '</td><td>' + number.format(d.cantidad * d.costoUnitario) +
            '</td><td>' + d.stockActual + '</td></tr>').join("") +
        '</tbody></table></div><p class="entry-total">Total <strong>' +
        number.format(entry.detalles.reduce((sum, d) => sum + d.cantidad * d.costoUnitario, 0)) +
        '</strong></p><p>El stock mostrado corresponde al momento de esta consulta.</p>';
}
function saleRows() {
    return [...document.querySelectorAll(".sale-row")];
}
function saleProducts() {
    return state.products.filter(p => p.activo && p.stockActual > 0 && (!p.fechaVencimiento || p.fechaVencimiento >= today()));
}
function updateSale() {
    const rows = saleRows();
    const selected = rows.map(row => row.querySelector("select").value);
    const available = saleProducts();
    let total = 0;
    rows.forEach((row, index) => {
        const select = row.querySelector("select");
        fillSelect(select, available.filter(p => !selected.some((id, i) => i !== index && id === String(p.id)))
            .map(p => ({ id: p.id, nombre: p.nombre + " · Stock disponible: " + p.stockActual })), "Selecciona un producto");
        const product = available.find(p => String(p.id) === select.value);
        const quantity = Number(row.querySelector("input").value);
        const price = product?.precioVenta ?? 0;
        const subtotal = Number.isFinite(quantity) ? price * quantity : 0;
        row.querySelector(".sale-price").textContent = number.format(price);
        row.querySelector(".sale-subtotal").textContent = number.format(subtotal);
        total += subtotal;
    });
    $("#sale-total").textContent = number.format(total);
}
function addSaleRow() {
    const row = document.createElement("div");
    row.className = "sale-row";
    row.innerHTML = '<label>Producto<select required></select></label>' +
        '<label>Cantidad<input type="number" min="1" max="2147483647" step="1" required value="1"></label>' +
        '<div class="sale-amount">Precio unitario<output class="sale-price"></output></div>' +
        '<div class="sale-amount">Subtotal<output class="sale-subtotal"></output></div>' +
        '<button type="button" class="secondary">Quitar</button>';
    row.querySelector("button").addEventListener("click", () => {
        if (saleRows().length === 1) return notice("La venta debe tener al menos un producto.", true);
        row.remove();
        updateSale();
    });
    row.addEventListener("input", updateSale);
    row.querySelector("select").addEventListener("change", updateSale);
    $("#sale-details").append(row);
    updateSale();
}
function renderSale(sale) {
    $("#sale-result").hidden = false;
    $("#sale-result").innerHTML = '<h2>Comprobante de venta #' + escapeHtml(sale.id) + '</h2><p>Fecha: ' + escapeHtml(sale.fechaVenta) +
        '</p><div class="table-wrap"><table><thead><tr><th>Producto</th><th>Cantidad</th><th>Precio unitario</th><th>Subtotal</th></tr></thead><tbody>' +
        sale.detalles.map(d => '<tr><td>' + escapeHtml(d.productoNombre) + '</td><td>' + escapeHtml(d.cantidad) +
            '</td><td>' + escapeHtml(number.format(d.precioUnitario)) + '</td><td>' + escapeHtml(number.format(d.subtotal)) + '</td></tr>').join("") +
        '</tbody></table></div><p class="entry-total">Total <strong>' + escapeHtml(number.format(sale.total)) + '</strong></p>';
    const alerts = sale.alertas || [];
    $("#sale-alerts").hidden = !alerts.length;
    $("#sale-alerts").innerHTML = alerts.length ? '<h2>Alertas después de la venta</h2><ul>' + alerts.map(a =>
        '<li>' + escapeHtml(a.productoNombre) + ' · Stock actual: ' + escapeHtml(a.stockActual) + ' · ' + escapeHtml(a.mensaje) + '</li>').join("") + '</ul>' : "";
}
const alertTypes = {
    SIN_STOCK: { label: "Sin stock", css: "empty" }, STOCK_BAJO: { label: "Stock bajo", css: "low" },
    POR_VENCER: { label: "Por vencer", css: "soon" }, VENCIDO: { label: "Vencido", css: "expired" }
};
function alertBadge(type) {
    const info = Object.hasOwn(alertTypes, type) ? alertTypes[type] : { label: type, css: "off" };
    return '<span class="badge ' + info.css + '">' + escapeHtml(info.label) + '</span>';
}
function renderAlerts() {
    const type = $("#alert-type").value;
    $("#alert-list").innerHTML = state.alerts.filter(a => !type || a.tipo === type).map(a =>
        '<li><strong>' + escapeHtml(a.productoNombre) + '</strong> ' + alertBadge(a.tipo) + '<p>' + escapeHtml(a.mensaje) +
        '</p><span>Stock / mínimo: ' + escapeHtml(a.stockActual) + ' / ' + escapeHtml(a.stockMinimo) +
        ' · Días para vencer: ' + escapeHtml(a.diasParaVencer ?? "Sin vencimiento") + '</span></li>').join("") || '<li>No hay alertas para este filtro.</li>';
}
function renderStock() {
    const search = $("#stock-search").value.trim().toLocaleLowerCase("es");
    const category = $("#stock-category").value;
    const onlyAlerts = $("#stock-only-alerts").checked;
    $("#stock-products").innerHTML = state.stock.filter(p => p.nombre.toLocaleLowerCase("es").includes(search) &&
        (!category || String(p.categoriaId) === category) && (!onlyAlerts || p.alertas.length)).map(p =>
        '<tr><td>' + escapeHtml(p.nombre) + '</td><td>' + escapeHtml(p.categoria) + '</td><td>' + escapeHtml(number.format(p.precioVenta)) +
        '</td><td>' + escapeHtml(p.stockActual) + ' / ' + escapeHtml(p.stockMinimo) + '</td><td>' + escapeHtml(p.fechaVencimiento || "Sin vencimiento") +
        '</td><td><div class="badges">' + (p.alertas.map(alertBadge).join("") || 'Sin alertas') + '</div></td></tr>').join("") ||
        '<tr><td colspan="6">No hay productos para este filtro.</td></tr>';
}
let stockRequest = 0;
async function refreshStock() {
    const request = ++stockRequest;
    $("#stock").setAttribute("aria-busy", "true");
    try {
        const [stock, summary, categories] = await Promise.all([api("stock"), api("alertas"), api("categorias")]);
        if (request !== stockRequest) return;
        state.stock = stock;
        state.alerts = summary.items;
        fillSelect($("#stock-category"), categories, "Todas las categorías");
        [["total", "total"], ["empty", "sinStock"], ["low", "stockBajo"], ["soon", "porVencer"], ["expired", "vencidos"]]
            .forEach(([id, key]) => { $("#alerts-" + id).textContent = summary[key]; });
        renderAlerts();
        renderStock();
    } finally {
        if (request === stockRequest) $("#stock").removeAttribute("aria-busy");
    }
}
$("#add-sale-row").addEventListener("click", addSaleRow);
$("#alert-type").addEventListener("change", renderAlerts);
["#stock-search", "#stock-category", "#stock-only-alerts"].forEach(selector => $(selector).addEventListener("input", renderStock));
$("#sale-form").addEventListener("submit", event => {
    event.preventDefault();
    submit(event.currentTarget, async () => {
        const lines = saleRows().map(row => ({ productoId: Number(row.querySelector("select").value), cantidad: Number(row.querySelector("input").value) }));
        if (!lines.length || lines.some(d => !d.productoId || !Number.isInteger(d.cantidad) || d.cantidad < 1 || d.cantidad > 2147483647))
            throw new Error("Selecciona un producto y una cantidad entera válida en cada fila.");
        if (new Set(lines.map(d => d.productoId)).size !== lines.length) throw new Error("Cada producto debe aparecer una sola vez en la venta.");
        const sale = await api("ventas", { detalles: lines });
        renderSale(sale);
        $("#sale-details").replaceChildren();
        addSaleRow();
        notice("Venta #" + sale.id + " registrada correctamente.");
        try { await refresh(); } catch { notice("La venta fue guardada, pero no se pudo actualizar el inventario. Pulsa Actualizar datos.", true); }
    });
});
async function submit(form, action) {
    const button = form.querySelector('[type="submit"]');
    if (button.disabled) return;
    button.disabled = true;
    notice("");
    try { await action(); } catch (error) { notice(error.message, true); }
    finally { button.disabled = false; }
}
document.querySelectorAll("[data-view]").forEach(button => button.addEventListener("click", () => showView(button.dataset.view)));
["#search", "#filter-category", "#filter-low"].forEach(selector => $(selector).addEventListener("input", renderProducts));
$("#refresh").addEventListener("click", () => {
    notice("");
    const operation = !$("#stock").hidden ? refreshStock() : refresh();
    operation.then(() => notice("Datos actualizados.")).catch(error => notice(error.message, true));
});
$("#add-detail").addEventListener("click", addDetail);
$("#product-form").addEventListener("submit", event => {
    event.preventDefault();
    const form = event.currentTarget;
    submit(form, async () => {
        const values = new FormData(form);
        const product = await api("productos", {
            nombre: values.get("nombre").trim(), categoriaId: Number(values.get("categoriaId")),
            precioVenta: Number(values.get("precioVenta")), stockMinimo: Number(values.get("stockMinimo")),
            fechaVencimiento: values.get("fechaVencimiento") || null
        });
        form.reset();
        showView("inventario");
        notice("Producto registrado: " + product.nombre + " (#" + product.id + ").");
        try { await refresh(); } catch { notice("El producto fue guardado, pero no se pudo actualizar el inventario. Pulsa Actualizar datos.", true); }
    });
});
$("#entry-form").addEventListener("submit", event => {
    event.preventDefault();
    const form = event.currentTarget;
    submit(form, async () => {
        const values = new FormData(form);
        const lines = details();
        if (new Set(lines.map(d => d.productoId)).size !== lines.length) throw new Error("Cada producto debe aparecer una sola vez en la entrada.");
        const entry = await api("entradas", {
            proveedorId: Number(values.get("proveedorId")), fechaEntrada: values.get("fechaEntrada"),
            nroDocumento: values.get("nroDocumento").trim() || null, detalles: lines
        });
        form.reset();
        form.elements.fechaEntrada.value = today();
        $("#details").replaceChildren();
        addDetail();
        updateTotal();
        showView("consulta");
        $("#lookup-form").elements.id.value = entry.id;
        renderEntry(entry);
        notice("Entrada #" + entry.id + " registrada. El stock se actualizó correctamente.");
        try { await refresh(); } catch { notice("La entrada fue guardada, pero no se pudo actualizar el inventario. Pulsa Actualizar datos.", true); }
    });
});
$("#lookup-form").addEventListener("submit", event => {
    event.preventDefault();
    const form = event.currentTarget;
    submit(form, async () => {
        $("#entry-result").hidden = true;
        renderEntry(await api("entradas/" + Number(form.elements.id.value)));
    });
});
$("#entry-form").elements.fechaEntrada.value = today();
$("#entry-form").elements.fechaEntrada.max = today();
$("#product-form").elements.fechaVencimiento.min = today();
addDetail();
addSaleRow();
refresh().catch(error => {
    $("#products").innerHTML = '<tr><td colspan="6">No se pudo cargar el inventario. Pulsa Actualizar datos para reintentar.</td></tr>';
    notice(error.message, true);
});
