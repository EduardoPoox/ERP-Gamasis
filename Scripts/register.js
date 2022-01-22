var reg = {
    init: function () {
        $("#register").on("submit", function (e) {
            e.preventDefault();
            var ds = new FormData(this);
            console.log(ds);
            $.ajax({
                data: ds,
                processData: false,
                contentType: false,
                type: "POST",
                url: Root + "Authenticate/Convert",
                beforeSend: function () {
                    Dialog.show("Espera un momento, estamos creando tu cuenta...", Dialog.type.progress);
                },
                success: function (response) {
                    console.log(response);
                    if (response == 1) {
                        Dialog.show("Se ha enviado un correo con el protocolo de activación, gracias por registrarte.");
                        setTimeout(function () {
                            location.href = Root + "Authenticate/Index";
                        }, 2000);
                    }
                    else if (response == 2)
                        Dialog.show("Hemos encontrado un error, intenta nuevamente o ponte en contacto con el administrador del sistema");
                    else
                        Dialog.show("Te has registrado, pero el correo no se pudo enviar, inicia sesión y sigue los pasos");

                },
                error: function (e) {
                    console.log(e);
                }
            });
        });
        console.log("script initialized...");
    }
};
$(function () {
    reg.init();
});