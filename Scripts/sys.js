var sys = {
    evts: {
        logout: function () {
            $.ajax({
                url: Root + "Authenticate/endSession",
                success: function () {
                    location.reload();
                }
            });
        }
    }
};