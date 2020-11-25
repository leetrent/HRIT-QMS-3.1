function executeDeleteNotification(notificationId) {
    console.log("[qms_notifications.js][executeDeleteNotification] => BEGIN:");
    const dataValue = { notificationId: notificationId }
    console.log("[qms_notifications.js][executeDeleteNotification] => (dataValue): ", dataValue);

    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: "/api/notification",
        contentType: "application/json",
        data: JSON.stringify(dataValue),
        error: function(jqXHR, testStatus, errorThrown) {
            console.log("[qms_notifications.js][executeDeleteNotification][error] => Error Encountered:");
            console.log("[qms_notifications.js][executeDeleteNotification][error] => (jqXHR): ", jqXHR);
            console.log("[qms_notifications.js][executeDeleteNotification][error] => (testStatus): ", testStatus);
            console.log("[qms_notifications.js][executeDeleteNotification][error] => (errorThrown): ", errorThrown);
        },
        success: function(result) {
            // console.log("[qms_notifications.js][executeDeleteNotification][success] => Call to 'url: '/api/notification' was successful");
            // console.log("[qms_notifications.js][executeDeleteNotification][success] => (result): ", result);
            removeNotificationCardFromDOM(notificationId)
        }
    });
    console.log("[qms_notifications.js][executeDeleteNotification] => END:");
}

function removeNotificationCardFromDOM(notificationId) {
    let deletedNotificationCard = document.getElementById(notificationId);
    if ( deletedNotificationCard) {
        deletedNotificationCard.remove();
    }
}

function deleteNotification(notificationId) {
    let deleteNotificationButton = document.getElementById("deleteNotificationButton");
    document.getElementById('staticBackdropButton').click();
    deleteNotificationButton.addEventListener("click", function(){
        console.log("[qms_notifications.js][deleteNotification] => In EventListener for delete button ...(notificationId):", notificationId);
        console.log(`[qms_notifications.js][deleteNotification] => Calling executeDeleteNotification(${notificationId})`);
        executeDeleteNotification(notificationId);
    }, false);
}

function markAllAsRead() {
    console.log("[qms_notifications.js][markAllAsRead] => BEGIN");
 
    let markAllAsReadModalButton = document.getElementById("markAllAsReadModalButton");
    markAllAsReadModalButton.click();

    let markAllAsReadWarningButton = document.getElementById("markAllAsReadWarningButton");
    markAllAsReadWarningButton.addEventListener("click", function() {
        let markAllAsReadForm = document.getElementById("markAllAsReadForm");
        console.log("[qms_notifications.js][markAllAsRead][markAllAsReadForm] =>", markAllAsReadForm);
        markAllAsReadForm.submit();       
    }, false);

    console.log("[qms_notifications.js][markAllAsRead] => END");
}

function deleteAllNotifications() {
    console.log("[qms_notifications.js][deleteAllNotifications] => BEGIN");

    let messageBody = document.getElementById("deleteNotificationBody");
    messageBody.textContent = "Are you sure you want to delete all your notifications?";

    document.getElementById('staticBackdropButton').click();
    let deleteNotificationButton = document.getElementById("deleteNotificationButton");

    deleteNotificationButton.addEventListener("click", function () {
        let deleteAllForm = document.getElementById("deleteAllForm");
        console.log("[qms_notifications.js][deleteAllNotifications][deleteAllForm] =>", deleteAllForm);
        deleteAllForm.submit();  
    }, false);

    console.log("[qms_notifications.js][deleteAllNotifications] => END");
}

/*
function deleteAllNotifications() {
    console.log("[qms_notifications.js][deleteAllNotifications] => BEGIN");

    let messageBody = document.getElementById("deleteNotificationBody");
    messageBody.textContent = "Are you sure you want to delete all your notifications?";

    document.getElementById('staticBackdropButton').click();
    let deleteNotificationButton = document.getElementById("deleteNotificationButton");

    deleteNotificationButton.addEventListener("click", function() {
        let cards = document.getElementsByClassName("card");
        let cardIds = [];

        for (let cardIndex = 0; cardIndex < cards.length; cardIndex++) {
            cardIds.push(cards[cardIndex].id);
        }

        let notificationCardContainer = document.getElementById("notificationCardContainer");
        if (notificationCardContainer) {
            notificationCardContainer.remove();
        }

        for ( let cardIdIndex = 0; cardIdIndex < cardIds.length; cardIdIndex++) {
            executeDeleteNotification(cardIds[cardIdIndex]);
        }

        let markAllDeleteAllButtonContainer = document.getElementById("markAllDeleteAllButtonContainer");
        if (markAllDeleteAllButtonContainer) {
            markAllDeleteAllButtonContainer.remove();
        }

        let notificationsHeader = document.getElementById("notificationsHeader");
        if (notificationsHeader) {
            notificationsHeader.remove();
        }
    }, false);

    console.log("[qms_notifications.js][deleteAllNotifications] => END");
}
*/


