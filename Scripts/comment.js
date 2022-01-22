var comment = {
    init: function (options) {
        if (options.hasOwnProperty("type"))
            comment.attr.type = options.type;
        else {
            console.warn("There must be a type, please check your code and send a type into the options");
            return;
        }
        if (options.hasOwnProperty("idrel"))
            comment.attr.idrel = options.idrel;
        else {
            console.warn("There must be a rel, please check your code and send a type into the options");
            return;
        }
        $("#comment").on("submit", function (e) {
            e.preventDefault();
            var ds = new FormData(this);
            ds.append("type", comment.attr.type);
            ds.append("idrel", rev.attr.reqid);
            ds.append("clientTime", moment().format('LLL'));
            $.ajax({
                url: Root + "Utils/AddComment",
                type: "POST",
                data: ds,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    Dialog.show("Espera un momento...", "Procesando operación", Dialog.type.progress);
                },
                success: function (res) {
                    if (res == 1) {
                        Dialog.show("Comentario añadido", "Operación exitosa", Dialog.type.progress);
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    }
                    else
                        Dialog.show("Hubo un error al añadir el comentario, intenta nuevamente.", "Operación fallida", Dialog.type.error);
                }
            });
        });
    },
    attr: {
        type: null
    }
};