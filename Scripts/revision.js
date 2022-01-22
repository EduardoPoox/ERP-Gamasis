var rev = {
    init: function (options) {
        $("#revision").on("submit", function (e) {
            e.preventDefault();
            var url = Root + "Utils/AddRevision";
            var ds = new FormData(this);
            ds.append("type", rev.attr.type);
            ds.append("idrel", rev.attr.reqid);
            ds.append("clientTime", moment().format('LLL'));
            $.ajax({
                url: url,
                data: ds,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (res) {
                    if (res == 1) {
                        Dialog.show("Se añadió correctamente la revisión", Dialog.type.progress);
                        setTimeout(function () {
                            location.reload();
                        }, 1500);
                    } else {
                        Dialog.show("Ocurrió un error al añadir la revisión", Dialog.type.success);
                    }
                },
                beforeSend: function () {
                    Dialog.show("Espera un momento...", Dialog.type.progress);
                }
            });
        });
        $("#end").on("click", function (e) {
            var message = "¿Estás seguro de que la incidencia fue solucionada?";
            if (rev.attr.type == 2)
                message = "¿Estás seguro de que el requerimiento ya fue añadido al sistema?";
            Dialog.show(message, Dialog.type.question);
            $(".sem-dialog").on("done", function () {
                var url = Root + "Incident/End";
                if (rev.attr.type == 2)
                    url = Root + "Requirement/End";
                $.ajax({
                    url: url,
                    data: { idrel: rev.attr.reqid, date: moment().format('LLL') },
                    beforeSend: function () {
                        Dialog.show("Espere un momento...", Dialog.type.progress);
                    },
                    success: function (res) {
                        if (res == 1) {
                            setTimeout(function () {
                                location.reload();
                            }, 1000);
                        }
                    }
                });
            });
        });
        $("#nextStep").on("click", function (e) {
            var message = "¿Estás seguro de que todo está correcto? Ya no se podrá regresar a la etapa de diseño nuevamente";

            Dialog.show(message, Dialog.type.question);
            $(".sem-dialog").on("done", function () {
                var url = Root + "Requirement/Next";
                $.ajax({
                    url: url,
                    data: { idrel: rev.attr.reqid, date: moment().format('LLL') },
                    beforeSend: function () {
                        Dialog.show("Espere un momento...", Dialog.type.progress);
                    },
                    success: function (res) {
                        if (res == 1) {
                            Dialog.show("Se notificará de que ya se debe pasar a la etapa de codificación...", Dialog.type.progress);
                            setTimeout(function () {
                                location.reload();
                            }, 1000);
                        }
                    }
                });
            });

        });
    },
    attr: {
        reqid: 0,
        type: 0
    },
    evts: {

    }
};
