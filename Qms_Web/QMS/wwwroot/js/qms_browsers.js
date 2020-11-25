$(document).ready(function(){
    console.log("[qms_browsers.js] => (navigator.userAgent).: ", navigator.userAgent);
    console.log("[qms_browsers.js] => (isSupportedBrowser()): ", isSupportedBrowser());

    if ( isSupportedBrowser() == false ) {
      document.getElementById("qmsWarning").style.display = "none";
      let warningContainer = document.getElementById("warningContainer");
      warningContainer.appendChild(createBrowserCard());
    }
  });
  function isSupportedBrowser() {
    return ( isEdge() || isChrome() );
  }
  function isEdge() {
    return (navigator.userAgent.indexOf("Edge") != -1 );
  }
  function isChrome() {
    return (navigator.userAgent.indexOf("Chrome") != -1 );
  }
  function createBrowserCard() {
    let card = document.createElement("div");
    card.setAttribute("class", "card text-center w-75 mx-auto mt-0");
    card.appendChild(createBrowserCardBody());
    return card;  
  }
  function createBrowserCardBody() {
    let cardBody = document.createElement("div");
    cardBody.setAttribute("class", "card-body");
    cardBody.appendChild(createBrowserCardTitle());
    cardBody.appendChild(createBrowserCardText());
    return cardBody;   
  }
  function createBrowserCardTitle() {
    let cardTitle = document.createElement("h5");
    cardTitle.setAttribute("class", "card-title");
    cardTitle.textContent = "SUPPORTED WEB BROWSERS";
    return cardTitle;
  }
  function createBrowserCardText()
  {
    let cardText = document.createElement("p");
    cardText.setAttribute("class", "card-text text-center");
    cardText.textContent = "HR/QMS supports the following web browsers: Google Chrome and Microsoft Edge.";
    return cardText;
  }