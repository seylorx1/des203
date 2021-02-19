function xhttpSuccess(xhttp) {
    return xhttp.readyState == 4 && xhttp.status == 200;
}

function populateSidebar() {
    let sidebar = document.getElementById("sidebar-content");

    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function() {
        if(xhttpSuccess(xhttp)) {

            let wikiFilesArray = [];
            let wikiFilesArrayIndex = 0;
            this.responseText.replace(/\r/g, "").split("\n").forEach(filePath => {
                if(new RegExp("(^wiki/)?(md$)").test(filePath)) {
                    wikiFilesArray.push(filePath);

                    let dirLength = filePath.lastIndexOf('/')+1;

                    let readableName = filePath
                        .substr(dirLength, filePath.length-dirLength-3)
                        .replace(/^_/g, "")
                        .replace(/_/g, " ");

                    let sidebarElement = document.createElement("a");

                    if(wikiFilesArrayIndex == 0) {
                        sidebarElement.classList.add("no-interact");
                        sidebarElement.innerHTML = "<b class=\"boldkerning\">"+readableName+"</b>"
                    }
                    else {
                        sidebarElement.innerHTML = readableName;
                        sidebarElement.href = "javascript:void(0)";
                        sidebarElement.onclick = function() {
                            alert(filePath);
                        };
                    }

                    sidebar.append(sidebarElement);
                    wikiFilesArrayIndex++;
                }
            });

            console.log(wikiFilesArray);
            //document.getElementById("targetDiv").textContent 
        }
    }
    xhttp.open("GET", "../allfiles.txt", true);
    xhttp.send();
}

populateSidebar();

function run() {

    //getMarkdownContent("/wiki/content/test.md");

    /*var text = doGET("/wiki/content/test.md"),
        target = document.getElementById('targetDiv'),
        converter = new showdown.Converter(),
        html = converter.makeHtml(text);
      
      target.innerHTML = html;*/
}