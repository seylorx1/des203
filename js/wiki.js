function xhttpSuccess(xhttp) {
    return xhttp.readyState == 4 && xhttp.status == 200;
}

class wikiPage {
    constructor(sidebarElement, readableName, filePath) {
        this.sidebarElement = sidebarElement;
        this.readableName = readableName;
        this.filePath = filePath;

        let that = this;
        this.sidebarElement.onclick = function() {
            selectWikiPage(that);
        }
    }

    //Returns: selected.
    updateSidebarElement(selected) {
        if(selected) {
            //Add no-interact if it's not already available.
            if(!this.sidebarElement.classList.contains("no-interact")) {
                this.sidebarElement.classList.add("no-interact");
            }

            //Don't render as link.
            if(this.sidebarElement.hasAttribute("href")) {
            this.sidebarElement.removeAttribute("href");
            }

            //Update and bolden the content of the sidebar element to the name of the wiki page.
            this.sidebarElement.innerHTML = "<b class=\"boldkerning\">"+ this.readableName +"</b>";
        }
        else {
            //Remove no-interact if it's available.
            if(this.sidebarElement.classList.contains("no-interact")) {
                this.sidebarElement.classList.remove("no-interact");
            }

            //Render as link.
            if(!this.sidebarElement.hasAttribute("href")) {
                this.sidebarElement.setAttribute("href", "javascript:void(0)");
            }

            //Update the content of the sidebar element to the name of the wiki page.
            this.sidebarElement.innerHTML = this.readableName;
        }

        return selected;
    }
}

var wikiPages = []

function selectWikiPage(page) {

    for(i = 0; i < wikiPages.length; i++) {
        if(wikiPages[i].updateSidebarElement(wikiPages[i].filePath == page.filePath)) {
            updateReader(wikiPages[i].filePath);
        }
    }
}

function updateReader(filePath) {
    //Get content of .md file.
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function() {
        if(xhttpSuccess(xhttp)) {
            let reader = document.getElementById("reader");
            let conv = new showdown.Converter();
            conv.setFlavor('github');
            reader.innerHTML = conv.makeHtml(xhttp.responseText);
        }
    }
    xhttp.open("GET", "../" + filePath);
    xhttp.send()
}

function populateSidebar() {
    let sidebar = document.getElementById("sidebar-content");

    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function() {
        if(xhttpSuccess(xhttp)) {

            this.responseText.replace(/\r/g, "").split("\n").forEach(filePath => {
                if(new RegExp("(^wiki/)?(md$)").test(filePath)) {


                    let dirLength = filePath.lastIndexOf('/')+1;

                    let readableName = filePath
                        .substr(dirLength, filePath.length-dirLength-3)
                        .replace(/^_/g, "")
                        .replace(/_/g, " ");

                    let sidebarElement = document.createElement("a");

                    wikiPages.push(new wikiPage(sidebarElement, readableName, filePath));

                    sidebar.append(sidebarElement);
                }
            });
            selectWikiPage(wikiPages[0]);
        }
    }
    xhttp.open("GET", "../allfiles.txt", true);
    xhttp.send();
}

populateSidebar();


let sidebarElement = document.getElementById("sidebar");
let sidebarInner = sidebarElement.offsetWidth - sidebarElement.clientWidth;

document.getElementById("sidebar__footer").setAttribute("style", "width: calc(12.5em - " + sidebarInner + "px)");