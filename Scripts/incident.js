var inc = {
    init: function () {
        $("#incident").on("submit", function (e) {
            e.preventDefault();
            var ds = new FormData(this);
            ds.append("createdAt", moment().format('LLL'));
            var url = Root + "Incident/Add";
            if (inc.attr.lstid != 0) {
                ds.append("_id", inc.attr.lstid);
                url = Root + "Incident/Update";
            }
            $("#mdlincident").on("hidden.bs.modal", function () {
                $("#incident").get(0).reset();
                inc.attr.lstid = 0;
            });
            $.ajax({
                url: url,
                data: ds,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (res) {
                    if (res > 0) {
                        Dialog.show("Se ha añadido correctamente, el siguiente paso es esperar a que sea revisado", "Incidencia añadida", Dialog.type.success);
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    } else
                        Dialog.show("Ha ocurrido un error, intenta nuevamente...", "Error en la operación", Dialog.type.error);
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
        console.log("script initialized...");
    },
    attr: {
        lstid: 0
    },
    evts: {
        info: function (upid) {
            Dialog.show("Espera un momento...", Dialog.type.progress);
            setTimeout(function () {
                location.href = Root + "Incident/Info?i=" + upid;
            }, 500);

            //$.ajax({
            //    url: Root + "Incident/Get",
            //    data: { upid: upid },
            //    success: function (res) {
            //        Dialog.hide();
            //        $("#dp").text(res.description);
            //        $("#n").text(res.name);
            //        $("#u").text(res.ubication);
            //        $("#st").text(res.ubication);
            //        $("#pro").text(res.progress);
            //        $("#mdlinfo").modal('show');

            //        var filesappend = "";

            //        $.each(res.files, function (i, value) {
            //            filesappend += "<div class='col-sm-4'>";
            //            filesappend += "<img class='img-circle' src='" + Root + "Incident/Image?i=" + value.id + "'>";
            //            filesappend += "</div'>";
            //        });
            //        $("#attaches").html(filesappend);
            //        console.log(res);
            //    },
            //    beforeSend: function () {
            //        Dialog.show("Espera un momento...", Dialog.type.progress);
            //    }
            //});
        },
        setValue: function (val) {
            $("#prospan").text(val + " %");
        },
        statuschange: function (val) {
            if (val == 4) {
                $("#progress").val("100");
                inc.evts.setValue("100");
                $("#progress").attr("disabled", "true");
            }
            else if (val == 1 || val == 2) {
                $("#progress").attr("disabled", "true");
                inc.evts.setValue("0");
                $("#progress").val("0");
            }
            else {
                $("#progress").removeAttr("disabled");
            }
        },
        delete: function (id) {
            Dialog.show("¿Está seguro(a) que quiere eliminar el requerimiento?", Dialog.type.question);
            $(".sem-dialog").on("done", function () {
                $.ajax({
                    url: Root + "Incident/Remove",
                    data: { idrel: id, date: moment().format('LLL') },
                    type: "GET",
                    beforeSend: function () {
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
                ds.append("id", inc.programmer.id);
                ds.append("updatedAt", moment().format('LLL'));

                $.ajax({
                    data: ds,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    url: Root + "Incident/Save",
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
                Dialog.show("¿Estás seguro de querer tomar la incidencia?", Dialog.type.question);
                $(".sem-dialog").on("done", function () {
                    inc.programmer.assing(inc.programmer.id);
                });
            });

        },
        assing: function (id) {
            $.ajax({
                url: Root + "Incident/Assing",
                data: { idinc: id, date: moment().format('LLL') },
                success: function (res) {
                    if (res == 1) {
                        Dialog.show("Te has asignado a esta incidencia, ahora podrás tener control sobre ella", Dialog.type.progress);
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
    revision: {
        init: function () {
            $("#revision").on("submit", function (e) {
                e.preventDefault();
                var ds = new FormData(this);
                $.ajax({
                    data: ds,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    url: Root + "Incident/AddRevision",
                    beforeSend: function () {
                        Dialog.show("Guardando revisión, espera...", Dialog.type.progress);
                    },
                    success: function (res) {
                        if (res == 1)
                            Dialog.show("Se ha guardado la información correctamente... se notificará al cliente de los cambios", Dialog.type.success);
                        else
                            Dialog.show("Ha ocurrido un error, intenta nuevamente", Dialog.type.success);
                    }
                });

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
                    url: Root + "Incident/AddFiles",
                    data: ds,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    beforeSend: function(){
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
        attr:{
            reqid: 0
        }
    }
};

$(function () {
    inc.init();
});