﻿async function confirmDeleteUser(formId, role) {
    let resp = await $.get("/api/admincount", null, null, "json");
    let adminsCount = resp.count;

    let msgHtml = "";
    let buttonsDivHtml = "";

    if (role == "admin" && adminsCount == 1) {
        msgHtml = "Nie można usunąć ostatniego administratora!";
        buttonsDivHtml = "<button onclick='closeDialog();' class='btn btn-light'>Zamknij</button>";

    }
    else {
        msgHtml = "Czy jesteś pewien, że chcesz usunąć użytkownika?";
        buttonsDivHtml = "<button type='submit' form='" + formId + "' class='btn btn-light' style='float: left;'>Tak</button><button onclick='closeDialog();' class='btn btn-light' style='float: right;'>Nie</button>";
    }

    showDialog(buttonsDivHtml, msgHtml);
}

function showDialog(buttonsDivHtml, msgHtml) {
    let divOverlay = document.createElement("div");
    divOverlay.id = "dialogOverlay";

    let div = document.createElement("div");
    div.id = "dialog";
    div.style = "display: none;"

    divOverlay.appendChild(div);
    document.body.appendChild(divOverlay);

    jQuery("#dialogOverlay").addClass("overlay");
    jQuery("#dialog").addClass("outerDialog");

    let dialogHeight = 200;
    let dialogYPos = Math.round((screen.availHeight - dialogHeight) / 2);
    jQuery("#dialog").css("height", dialogHeight + "px");
    jQuery("#dialog").css("top", dialogYPos + "px");

    jQuery("#dialog").html("<div class='innerDialog'><div class='mb-4'>" + msgHtml + "</div><div class='buttonsDiv'>" + buttonsDivHtml + "</div></div>");
    jQuery("#dialog").show(400);
}

function closeDialog() {
    jQuery("#dialog").hide(400)
    setTimeout(function () {
        document.body.removeChild(document.getElementById("dialogOverlay"));
    }, 400);
}