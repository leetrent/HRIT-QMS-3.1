function addComment() {
    console.log("[qms_ca_comments.js][addComment] => BEGIN:");

    const commentPost = {
        userId:  $("#UserId").val(),
        correctiveActionId: $("#CorrectiveActionIdForAddComment").val(),
        comment: $("#caComment").val(),
    };

    console.log("[qms_ca_comments.js][addComment] => (commentPost): ", commentPost);

    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: "/api/ca/comment",
        contentType: "application/json",
        data: JSON.stringify(commentPost),
        error: function(jqXHR, testStatus, errorThrown) {
            console.log("[qms_ca_comments.js][addComment][error] => Error Encountered:");
            console.log("[qms_ca_comments.js][addComment][error] => (jqXHR): ", jqXHR);
            console.log("[qms_ca_comments.js][addComment][error] => (testStatus): ", testStatus);
            console.log("[qms_ca_comments.js][addComment][error] => (errorThrown): ", errorThrown);
            console.log("[qms_ca_comments.js][addComment][error] => (commentPost): ", commentPost);
        },
        success: function(result) {
            console.log("[qms_ca_comments.js][addComment][success] => Call to '/api/ca/comment/create' was successful");
            console.log("[qms_ca_comments.js][addComment][success] => (result): ", result);
            document.getElementById("caComment").value='';
            getComments($("#CorrectiveActionIdForAddComment").val());
        }
    });
    console.log("[qms_ca_comments.js][addComment] => END:");
}
function getComments(correctiveActionId) {
    console.log("[qms_ca_comments.js][getComments] => BEGIN:");
    console.log("[qms_ca_comments.js][getComments] => (correctiveActionId): ", correctiveActionId);

    $.ajax({
        type: "GET",
        url: "/api/ca/comment",
        data: {
            id: correctiveActionId
        },
        cache: false,
        success: function(data) {
            console.log("[qms_ca_comments.js][getComments][success] => Call to '/api/ca/comment/RetrieveAll' was successful");
             let cardContainer = document.getElementById("cardContainer");
             //cardContainer.remove();
             cardContainer.querySelectorAll('*').forEach(n => n.remove());
            $.each(data, function(key, item) {
                 cardContainer.appendChild(createCard(item));
            })
            document.getElementById("view-comments-tab").click();
        }
    });

    console.log("[qms_ca_comments.js][getComments] => END:");
}

function createCommentCard(commentInfo) {
    let cardHeader = "<div class=\"card-header\">" + commentInfo.orgLabel     + "</div>";
    let cardTitle  = "<h5 class=\"card-title\">"   + commentInfo.displayName  + "</h5>";
    let cardText   = "<p class=\"card-text\">"     + commentInfo.message      + "</p>";
    let cardFooter = "div class=\"card-footer\">"  + commentInfo.dateCreated  + "</div>";

    console.log("--------------------------------------------------------------------------------");
    console.log(cardHeader);
    console.log(cardTitle);
    console.log(cardText);
    console.log(cardFooter);
    console.log("--------------------------------------------------------------------------------");
}
function createCardHeader(orgLabel) {
    let cardHeader = document.createElement("div");
    cardHeader.setAttribute("class", "card-header");
    cardHeader.textContent = orgLabel;
    return cardHeader;
}
function createCardTitle(displayName) {
    let cardTitle = document.createElement("h5");
    cardTitle.setAttribute("class", "card-title");
    cardTitle.textContent = displayName;
    return cardTitle;
}
function createCardText(message) {
    let cardText = document.createElement("p");
    cardText.setAttribute("class", "card-text");
    cardText.textContent = message;
    return cardText;
}
function createCardFooter(dateCreated) {
    let cardFooter = document.createElement("div");
    cardFooter.setAttribute("class", "card-footer");
    cardFooter.textContent = dateCreated;
    return cardFooter;
}
function createCardBody(commentInfo) {
    let cardBody = document.createElement("div");
    cardBody.setAttribute("class", "card-body");
    cardBody.appendChild(createCardTitle(commentInfo.displayName));
    cardBody.appendChild(createCardText(commentInfo.message));
    return cardBody;
}
function createCard(commentInfo) {
    let card = document.createElement("div");
    card.setAttribute("class", "card mb-3");
    card.style.maxWidth = "18rem";
    card.appendChild(createCardHeader(commentInfo.orgLabel));
    card.appendChild(createCardBody(commentInfo));
    card.appendChild(createCardFooter(commentInfo.dateCreated));
    return card;
}

