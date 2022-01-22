var log = {
    init: function () {
        $("#login").on("submit", function (e) {
            e.preventDefault();
            var ds = new FormData(this);
            ds.append("startedAt", moment().format('LLL'));
            $.ajax({
                data: ds,
                processData: false,
                contentType: false,
                url: Root + "Authenticate/Start",
                type: 'POST',
                beforeSend: function () {
                    Dialog.show("Espera un momento, estamos autenticandote...", Dialog.type.progress);
                },
                success: function (res) {
                    console.log(res);
                    if (res == 0)
                        Dialog.show("El usuario o contraseña son incorrectos", Dialog.type.error);
                    else if (res == 2) { //Means you are in on any other db but not in GPM
                        Dialog.show("Porfavor, sigue nuestras instrucciones...", Dialog.type.progress);
                        setTimeout(function () {
                            location.href = Root + "Authenticate/Create";
                        }, 1500);
                    }
                    else if (res == 3) { //Inactive account
                        Dialog.show("Tu usuario ya se encuentra registrado, pero tu cuenta se encuentra inactiva, sigue nuestras instrucciones para activarla...", Dialog.type.progress);
                        setTimeout(function () {
                            location.href = Root + "Authenticate/ActivateAccount";
                        }, 1500);
                    }
                    else {
                        Dialog.show("Se inició sesión correctamente, te redirigiremos...", Dialog.type.progress);
                        setTimeout(function () {
                            location.href = Root;
                        }, 1500);
                    }
                },
                error: function (xhr) { console.log(xhr); }
            });
        });
        console.log("login script initialized correctly...");

    },
    evts: {
        logout: function () {

        }
    }
};
$(function () {
    log.init();
});