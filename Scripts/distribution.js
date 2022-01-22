var dist = {
	init: function () {
		$("#distlist").on("submit", function (e) {
			e.preventDefault();
			var url = Root + "Users/AddDist";
			var ds = new FormData(this);
			ds.append("date", moment().format('LLL'));
			var members = $("#imembers tbody");
			var dlmembers = new Array();
			members.find('tr').each(function (i, el) {
				var id = $(this).data("dlid");
				var row = $(this).find('td');
				var member = {
					id: id,
					idmember: 0,
					idrel: 0,
					mail: "",
					comesfrom: "",
					cellphone: "",
					name: ""
				};
				dlmembers.push(member);
			});
			if (dlmembers.length > 0)
				ds.append('dlmembers', JSON.stringify(dlmembers));
			else {
				Dialog.show("No se han añadido miembros, al menos debes seleccionar uno", "Información", Dialog.type.success);
				return;
			}
			if (dist.attr.lastid != 0) {
				ds.append("_id", dist.attr.lastid);
				url = Root + "Users/UpdateDist";
			}
			$.ajax({
				url: url,
				processData: false,
				contentType: false,
				type: 'POST',
				data: ds,
				error: function (xhr) { console.log(xhr); },
				beforeSend: function () {
					Dialog.show("Espera un momento...", "Información", Dialog.type.progress);
				},
				success: function (res) {
					if (res == 1) {
						Dialog.show("Información añadida exitosamente...", "Información", Dialog.type.progress);
						setTimeout(function () {
							location.reload();
						}, 2000);
					} else {
						Dialog.show("Hubo un error al intentar añadir la información...", "Información", Dialog.type.success);
					}
				}
			});
		});
		$('#mdllist').on('hidden.bs.modal', function () {
		    dist.attr.lastid = 0;
		    document.getElementById("distlist").reset();
		    $("#imembers").find('tbody').html(null);
		});
		$('#mdlmembersinfo').on('hidden.bs.modal', function () {
		    $("#iimembers").find('tbody').html(null);
		});
	},
	attr: {
		lastid: 0
	},
	evts: {
		add: function (element) {
			var member = element.value;
			var selectedText = $("#member option:selected").text();
			if (member.trim() == "") {
				return;
			}
			if (!$("[data-dlid=" + member + "]").length) {
				$("#imembers").find("tbody").append("<tr data-dlid=\"" + member + "\">" +
					 "<td>" + selectedText + "<input type=\"hidden\" name=\"scdes\" value=\"" + selectedText + "\"></td>" +
					 "<td><button type=\"button\" onclick=\"dist.evts.remove(this)\" class=\"btn btn-xs btn-danger\"><i class=\"fa fa-trash-o\"></i> Eliminar</button></td></tr>");
				$("#imembers").parent().removeClass("hide");
			}
			else {
				Dialog.show("Este usuario, ya se encuentra en esta lista de distribución", Dialog.type.error);
			}
		},
		remove: function (element) {
			Dialog.show("Estás seguro de querer eliminar este usuario de la lista de distribución?", Dialog.type.question);
			$(".sem-dialog").on("done", function () {
				$(element).parent().parent().remove();
				if (!$("#imembers").find("tbody tr").length) {
					$("#imembers").parent().addClass("hide");
				}
			});
		},
		info: function (id) {
			dist.attr.lastid = id;
			$.ajax({
				url: Root + "Users/GetDist",
				data: { id: id },
				type: "GET",
				success: function (res) {
				    $("#name").val(res.name);
				    $("#description").val(res.description);
				    var tbl = "";
				    $.each(res.members, function (i, val) {
				        tbl += "<tr data-dlid=\"" + val.idmember + "\">";
				        tbl += "<td>" + val.name + " - " + val.mail + "</td>";
				        tbl += "<td><button type=\"button\" onclick=\"dist.evts.remove(this)\" class=\"btn btn-xs btn-danger\">Eliminar</button></td>";
				    });
				    tbl += "</tr>";
				    $("#imembers").find("tbody").html(tbl);
				    $("#imembers").parent().removeClass("hide");
				    $("#mdllist").modal("show");
					Dialog.hide();
				},
				beforeSend: function () {
					Dialog.show("Espera un momemnto...", "Información", Dialog.type.progress);
				}
			});
		},
		showmembers: function (id) {
		    $.ajax({
		        url: Root + "Users/GetDist",
		        data: { id: id },
		        type: "GET",
		        success: function (res) {
		            $("#mdltiimember").text(res.name);
		            var tbl = "";
		            $.each(res.members, function (i, val) {
		                tbl += "<tr>";
		                tbl += "<td>" + val.name + " - " + val.mail + "</td>";
		            });
		            tbl += "</tr>";
		            $("#iimembers").find("tbody").html(tbl);
		            $("#iimembers").parent().removeClass("hide");
		            $("#mdlmembersinfo").modal("show");
		            Dialog.hide();
		        },
		        beforeSend: function () {
		            Dialog.show("Espera un momento...", "Información", Dialog.type.progress);
		        }
		    });
		}
	}
};
$(function () {
	dist.init();
});