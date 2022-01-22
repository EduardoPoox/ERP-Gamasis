var act = {
    init: function () {
        console.info("script initialized...");
        $("#activate").on("submit", function (e) {
            e.preventDefault();
            var url = Root + "Authenticate/ActivateMail";
            var ds = new FormData(this);
            $.ajax({
                url: url,
                data: ds,
                processData: false,
                contentType: false,
                type: 'POST',
                beforeSend: function () {
                    Dialog.show("Espera un momento...", "Procesando operación", Dialog.type.progress);
                },
                success: function (res) {
                    if (res == 1) {
                        Dialog.show("Revisa tu bandeja de entrada del correo añadido, y tendrás instrucciones de como activar tu cuenta.");
                    } else
                        Dialog.show("Hubo un error al enviar el mensaje, intenta nuevamente.");
                }
            });
        });
    }
}

$(function () {
    act.init();
});