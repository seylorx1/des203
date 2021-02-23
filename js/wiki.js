var wikiPages = []
//Do not set directly.
var selectedWikiPage;

class wikiPage {
    constructor(sidebarElement, filePath) {
        this.sidebarElement = sidebarElement;
        this.filePath = filePath;

        //Generate the readable name.
        let dirLength = filePath.lastIndexOf('/') + 1;
        this.queryID = filePath.substr(dirLength, filePath.length - dirLength - 3);
        this.readableName = this.queryID.replace(/^_/g, "").replace(/_/g, " ");

        let that = this;
        this.sidebarElement.onclick = function () {
            displayWikiPage(that);
            updateSearchQuery(that);
        }
    }

    //Returns: selected.
    updateSidebarElement(selected) {
        if (selected) {
            selectedWikiPage = this;

            //Add no-interact if it's not already available.
            if (!this.sidebarElement.classList.contains("no-interact")) {
                this.sidebarElement.classList.add("no-interact");
            }

            //Don't render as link.
            if (this.sidebarElement.hasAttribute("href")) {
                this.sidebarElement.removeAttribute("href");
            }

            //Update and bolden the content of the sidebar element to the name of the wiki page.
            this.sidebarElement.innerHTML = "<b class=\"boldkerning\">" + this.readableName + "</b>";
        }
        else {
            //Remove no-interact if it's available.
            if (this.sidebarElement.classList.contains("no-interact")) {
                this.sidebarElement.classList.remove("no-interact");
            }

            //Render as link.
            if (!this.sidebarElement.hasAttribute("href")) {
                this.sidebarElement.setAttribute("href", "javascript:void(0)");
            }

            //Update the content of the sidebar element to the name of the wiki page.
            this.sidebarElement.innerHTML = this.readableName;
        }

        return selected;
    }
}

//Updates the sidebar styles and uses AJAX to update the page content.
function displayWikiPage(page) {
    for (i = 0; i < wikiPages.length; i++) {
        if (wikiPages[i].updateSidebarElement(wikiPages[i] == page)) {
            updateReader(wikiPages[i].filePath);
        }
    }
}
function displayWikiPageFromQuery(queryID) {
    for (i = 0; i < wikiPages.length; i++) {
        if (wikiPages[i].updateSidebarElement(wikiPages[i].queryID == queryID)) {
            updateReader(wikiPages[i].filePath);
        }
    }
}

function getAnchorTagFromURL(url) {
    if(url.includes("#")) {
        let anchorTagIndex = url.lastIndexOf("#") + 1;
        return url.substr(anchorTagIndex, url.length - anchorTagIndex);
    }
}
function removeAnchorTagFromURL(url) {
    if (url.includes("#")) {
        return url.substr(0, url.lastIndexOf("#") );
    }
    return url;
}

function createJumpLinks(childNodes) {
    //Only add the link if the browser supports copying to the clipboard.
    if (navigator.clipboard) {
        let tags = [
            "H1",
            "H2",
            "H3",
            "H4",
            "H5",
            "H6"
        ];
        for (i = 0; i < childNodes.length; i++) {
            for (j = 0; j < tags.length; j++) {
                let currentChildNode = childNodes[i];

                if (currentChildNode.nodeName == tags[j]) {
                    currentChildNode.setAttribute("class", "reader--header");
                    let jumpLinkElement = document.createElement("p");
                    jumpLinkElement.setAttribute("class", "material-icons link")
                    jumpLinkElement.textContent = "link";



                    jumpLinkElement.onclick = function () {
                        try {
                            navigator.clipboard.writeText(
                                removeAnchorTagFromURL(document.location.href) +
                                "#" +
                                currentChildNode.getAttribute("id"));
                        }
                        catch (err) {
                            console.error("Failed to copy text!", err);
                        }
                    }

                    currentChildNode.appendChild(jumpLinkElement);
                }
            }
        }
    }
}

function xhttpSuccess(xhttp) {
    return xhttp.readyState == 4 && xhttp.status == 200;
}

function updateReader(filePath) {
    //Get content of .md file.
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttpSuccess(xhttp)) {
            let reader = document.getElementById("reader");
            let conv = new showdown.Converter();
            conv.setFlavor('github');

            let readerHTML = conv.makeHtml(xhttp.responseText);
            reader.innerHTML = readerHTML;

            createJumpLinks(reader.childNodes);

            jumpToURLAnchorTag();
        }
    }
    xhttp.open("GET", "../" + filePath);
    xhttp.send()
}

function updateSearchQuery(page) {
    let url = new URL(removeAnchorTagFromURL(window.location.href));
    if (page !== wikiPages[0]) {
        url.searchParams.set("page", page.queryID);
    }
    else {
        url.searchParams.delete("page");
    }
    window.history.pushState({}, "", url);
}
function openPageFromSearchQuery() {
    let searchParams = new URLSearchParams(window.location.search);
    let target = searchParams.get("page");

    if (target !== null) {
        //Check to see if target is contained
        displayWikiPageFromQuery(target);
    }
    else {
        displayWikiPage(wikiPages[0]);
    }
}

function jumpToURLAnchorTag() {
    let anchorTag = getAnchorTagFromURL(document.location.href);
    if(anchorTag != null) {
        //Anchor tag exists in URL
        let element = document.getElementById(anchorTag);
        if(element != null) {
            console.log(element);
            //Element for anchor tag exists
            document.getElementById("reader").scrollTo(0, element.offsetTop);
        }
        else {
            //Remove invalid anchor tag from url
            document.location.href = removeAnchorTagFromURL(document.location.href);
        }
    }
}

//Populates the wikiPages array and updates the sidebar.
function populateWikiPages() {
    let sidebar = document.getElementById("sidebar-content");

    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttpSuccess(xhttp)) {

            this.responseText.replace(/\r/g, "").split("\n").forEach(filePath => {
                if (new RegExp("(^wiki/)?(md$)").test(filePath)) {

                    let sidebarElement = document.createElement("a");
                    wikiPages.push(new wikiPage(sidebarElement, filePath));

                    sidebar.append(sidebarElement);
                }
            });

            openPageFromSearchQuery();
        }
    }
    xhttp.open("GET", "../allfiles.txt", true);
    xhttp.send();
}

function updateSidebarWidth() {
    let sidebarElement = document.getElementById("sidebar");
    let sidebarInner = sidebarElement.offsetWidth - sidebarElement.clientWidth;
    document.getElementById("sidebar__footer").setAttribute("style",
        "width: calc(12.5em - " + sidebarInner + "px); " +
        "max-width: calc(40vw - " + sidebarInner + "px);");
}


populateWikiPages();
updateSidebarWidth();