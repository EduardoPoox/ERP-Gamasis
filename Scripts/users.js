var us = {
    init: function () {
        $("#user").on("submit", function (e) {
            e.preventDefault();
            if (us.attr.lastid == 0)
                return;
            var ds = new FormData(this);
            ds.append("_id", us.attr.lastid);
            var assignations = $("#iassignations tbody");
            var dlassig = new Array();
            assignations.find('tr').each(function (i, el) {
                var id = $(this).data("id");
                var row = $(this).find('td');
                var assignation = {
                    value: row.eq(0).text(),
                    type: id,
                    typestring: "",
                    accountid: 0,
                    accountname: "",
                    id: 0
                };
                dlassig.push(assignation);
            });
            console.log(dlassig);
            if (dlassig.length > 0)
                ds.append('dlassig', JSON.stringify(dlassig));
            else {
                Dialog.show("No se han añadido asignaciones, al menos debes seleccionar una", "Información", Dialog.type.success);
                return;
            }

            $.ajax({
                url: Root + "Users/Update",
                type: "POST",
                processData: false,
                contentType: false,
                data: ds,
                beforeSend: function () {
                    Dialog.show("Espera un momento...", "Información", Dialog.type.progress);
                },
                success: function (res) {
                    if (res > 0) {
                        Dialog.show("Se actualizó el perfil de manera exitosa, espera un momento...", "Información", Dialog.type.progress);
                        setTimeout(function () {
                            location.reload();
                        }, 1500);
                    } else {
                        Dialog.show("Ocurrió un error al intentar actualizar el perfil", "Información", Dialog.type.success);
                    }
                }
            });
        });
        $('#mdluser').on('hidden.bs.modal', function () {

        });
    },
    attr: {
        lastid: 0
    },
    evts: {
        add: function () {
            var type = $("#ntype").val();
            var assignation = $("#nasign").val();
            if (type.trim() == "") {
                Dialog.show("Selecciona un tipo", Dialog.type.error);
                return;
            }
            if (assignation.trim() == "") {
                Dialog.show("Selecciona una asignación", Dialog.type.error);
                return;
            }
            if (!$("[data-idrel='" + type + "']").length) {
                $("#iassignations").find("tbody").append("<tr data-idrel=\"" + assignation + "\" data-id=\"" + type + "\">" +
                     "</td><td>" + assignation + "<input type=\"hidden\" name=\"udassign\" value=\"" + assignation + "\"></td>" +
                     "</td><td>" + (type == 1 ? "Módulo" : "Desarrollador") + "<input type=\"hidden\" name=\"udtype\" value=\"" + type + "\"></td>" +
                     "<td><button type=\"button\" onclick=\"us.evts.remove(this)\" class=\"btn btn-xs btn-danger\"><i class=\"fa fa-trash-o\"></i> Eliminar</button></td></tr>");
                $("#iassignations").parent().removeClass("hide");
                $("#nasign").val("");
                $("#ntype").val("");
            }
            else {
                Dialog.show("La asignación, ya fue añadida", Dialog.type.error);
            }
        },
        remove: function (element) {
            Dialog.show("Estás seguro de querer eliminar esta asignación?", Dialog.type.question);
            $(".sem-dialog").on("done", function () {
                $(element).parent().parent().remove();
                if (!$("#iassignations").find("tbody tr").length) {
                    $("#iassignations").parent().addClass("hide");
                }
            });
        },
        info: function (id) {
            us.attr.lastid = id;
            $.ajax({
                url: Root + "Users/GetOne",
                data: { id: id },
                type: "GET",
                success: function (res) {
                    console.log(res);
                    $("#name").val(res.data.name);
                    $("#lastname").val(res.data.lastname);
                    $("#mail").val(res.data.email);
                    $("#cellphone").val(res.data.cellphone);
                    $("#nrol").val(res.rolObject.id);

                    var tbl = "";
                    $.each(res.assignations, function (i, val) {
                        tbl += "<tr data-idrel=\"" + val.value + "\" data-id=\"" + val.type + "\">";
                        tbl += "<td>" + val.value + "</td>";
                        tbl += "<td>" + (val.type == 1 ? "Módulo" : "Desarrollador") + "</td>";
                        tbl += "<td><button type=\"button\" onclick=\"us.evts.remove(this)\" class=\"btn btn-xs btn-danger\">Eliminar</button></td>";
                    });
                    tbl += "</tr>";
                    $("#iassignations").find("tbody").html(tbl);
                    $("#iassignations").parent().removeClass("hide");
                    $("#mdluser").modal("show");
                    Dialog.hide();
                },
                beforeSend: function () {
                    Dialog.show("Espera un momemnto...", "Información", Dialog.type.progress);
                }
            });
        },
        typeChange: function (type) {
            var options = "<option value=\"\">-Seleccionar</option>";
            if (type == 1) {
                options += "<option value=\"Incident\">- Acceso al módulo de incidencias</option>";
                options += "<option value=\"Requirement\">- Acceso al módulo de requerimientos</option>";
                options += "<option value=\"Users\">- Acceso al módulo de usuarios</option>";
                $("#nasign").html(options);
            } else {
                options += "<option value=\"Alave\">- Acceso a requerimientos e incidencias de la empresa ALAVE</option>";
                $("#nasign").html(options);
            }
        }
    }
};
$(function () {
    us.init();
});

