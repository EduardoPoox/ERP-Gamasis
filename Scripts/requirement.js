var req = {
    init: function (type) {
        try {
            $('[data-fancybox="gallery"]').fancybox({
                buttons: [
                    'slideShow',
                    'thumbs',
                    'zoom',
                    'fullScreen',
                    'download',
                    'close']
            });
        } catch (ex) { console.warn(ex); }
        if (type != undefined) {
            if (type == "Temp") {
                $("#requirement").on("submit", function (e) {
                    var url = Root + "Requirement/Temp";
                    var ds = new FormData(this);
                    $.ajax({
                        url: url,
                        data: ds,
                        processData: false,
                        contentType: false,
                        type: 'POST',
                        success: function (res) {
                            console.log(res);
                            if (res != 1)
                                return;
                            (res == 1) ? location.href = Root + "Requirement/Create" : Dialog.show("Ha ocurrido un error, intenta otra vez", Dialog.type.error);
                        },
                        beforeSend: function () {
                            Dialog.show("Espera un momento...", Dialog.type.progress);
                        },
                        error: function (xhr) {
                            Dialog.hide();
                            console.error(xhr.responseText);
                        }
                    });
                });
            } else {
                $("#requirement").on("submit", function (e) {
                    e.preventDefault();
                    var url = Root + "Requirement/Add";
                    var ds = new FormData(this);
                    //console.log($("#source").val().replace(/\r/g, "").replace(/\n/g, ""));
                    //return;
                    ds.append("createdAt", moment().format('LLL'));
                    ds.append("templatetext", $("#source").val().replace(/\r/g, "").replace(/\n/g, ""));
                    $.ajax({
                        url: url,
                        data: ds,
                        processData: false,
                        contentType: false,
                        type: 'POST',
                        success: function (res) {
                            if (res != 0) {
                                Dialog.show("Se ha añadido la solicitud de módulo correctamente, el siguiente paso es esperar indicaciones", Dialog.type.success)
                                setTimeout(function () { location.href = Root + "Requirement/Index"; }, 1500);
                            } else
                                Dialog.show("Ha ocurrido un error, intenta otra vez", Dialog.type.error);
                        },
                        beforeSend: function () {
                            Dialog.show("Espera un momento...", Dialog.type.progress);
                        },
                        error: function (xhr) {
                            Dialog.hide();
                            console.error(xhr.responseText);
                        }
                    });
                });
            }
        }
        else {
            console.warn("No se especificó un tipo...");
            return;
        }
    },
    evts: {
        typechange: function (t) {
            if (t == '1')
                $("#dcolumns").removeClass('hide');
            else {
                $("#dcolumns").removeClass('hide');
                $("#dcolumns").val('');
                $("#dcolumns").addClass('hide');
            }
        },
        info: function (upid) {
            if (upid == undefined)
                console.error("there was not a upid");
            (upid != 0) ? location.href = Root + "Requirement/Info?i=" + upid : console.error("an error has ocurred");
        },
        setValue: function (val) {
            $("#prospan").text(val + " %");
        },
        statuschange: function (val) {
            if (val == 4 || val == 34) {
                $("#progress").val("100");
                req.evts.setValue("100");
                $("#progress").attr("disabled", "true");
            }
            else if (val == 1 || val == 2) {
                $("#progress").attr("disabled", "true");
                req.evts.setValue("0");
                $("#progress").val("0");
            }
            else {
                $("#progress").removeAttr("disabled");
                req.evts.setValue("0");
                $("#progress").val("0");
            }
        },
        attr: {
            htm: null
        },
        delete: function (id) {
            Dialog.show("¿Está seguro(a) que quiere eliminar el requerimiento?", Dialog.type.question);
            $(".sem-dialog").on("done", function () {
                $.ajax({
                    url: Root + "Requirement/Remove",
                    data: { idrel: id, date: moment().format('LLL') },
                    type: "GET",
                    beforeSend: function(){
                        Dialog.show("Espera un momento...", "Operación", Dialog.type.progress);
                    },
                    success: function (res) {
                        if (res > 0) {
                            Dialog.show("Se eliminó el requerimiento exitosamente, espera un momento", "Operación exitosa", Dialog.type.progress);
                            setTimeout(function () {
                                location.reload();
                            }, 1000);
                        } else
                            Dialog.show("Ocurrió un error al eliminar el requerimiento", "Operación exitosa", Dialog.type.error);
                    }
                });
            });
        }
    },
    programmer: {
        id: 0,
        init: function () {
            $("#update").on("submit", function (e) {
                e.preventDefault();
                var ds = new FormData(this);
                ds.append("id", req.programmer.id);
                ds.append("updatedAt", moment().format('LLL'));
                $.ajax({
                    data: ds,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    url: Root + "Requirement/Save",
                    beforeSend: function () {
                        Dialog.show("Guardando información, espera...", Dialog.type.progress);
                    },
                    success: function (res) {
                        console.log(res);
                        if (res == 1)
                            Dialog.show("Se ha guardado la información correctamente... se notificará al cliente de los cambios", Dialog.type.success);
                        else
                            Dialog.show("Ha ocurrido un error, intenta nuevamente", Dialog.type.success);
                    },
                    error: function (xhr) {
                        Dialog.hide();
                        console.error(xhr.responseText);
                    }
                });
            });
            $("#tr").on("click", function (e) {
                Dialog.show("¿Estás seguro de querer tomar el requerimiento?", Dialog.type.question);
                $(".sem-dialog").on("done", function () {
                    console.log(req.programmer.id);
                    req.programmer.assing(req.programmer.id);
                });
            });
        },
        assing: function (id) {
            $.ajax({
                url: Root + "Requirement/Assing",
                data: { idinc: id, date: moment().format('LLL') },
                success: function (res) {
                    if (res == 1) {
                        Dialog.show("Te has asignado a este módulo, ahora podrás tener control sobre ella", Dialog.type.progress);
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    } else {
                        Dialog.show("Ha ocurrido un error, intenta nuevamente...", "Error en la operación", Dialog.type.error);
                    }
                },
                type: "GET"
            });

        }
    },
    files: {
        init: function () {
            $("#addfiles").on("submit", function (e) {
                e.preventDefault();
                var ds = new FormData(this);
                ds.append("idrel", inc.files.attr.reqid);
                $.ajax({
                    url: Root + "Requirement/AddFiles",
                    data: ds,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    beforeSend: function () {
                        Dialog.show("Espera un momento...", "Operación", Dialog.type.progress);
                    },
                    success: function (res) {
                        if (res > 0) {
                            Dialog.show("Se ha añadido los archivos correctamente, espera un momento...", "Incidencia actualizada", Dialog.type.progress);
                            setTimeout(function () {
                                location.reload();
                            }, 1000);
                        } else
                            Dialog.show("Ha ocurrido un error, intenta nuevamente...", "Error en la operación", Dialog.type.error);
                    }
                });

            });
        },
        attr: {
            reqid: 0
        }
    }
};
