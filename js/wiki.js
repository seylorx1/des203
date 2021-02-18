function xhttpSuccess(xhttp) {
    return xhttp.readyState == 4 && xhttp.status == 200;
}

function getFilesFromGitHub() {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function() {
        if(xhttpSuccess(xhttp)) {

            let wikiFilesArray = [];
            this.responseText.replace(/\r/g, "").split("\n").forEach(filePath => {
                if(new RegExp("(^wiki/)?(md$)").test(filePath)) {
                    wikiFilesArray.push(filePath);
                }
            });

            console.log(wikiFilesArray);
            //document.getElementById("targetDiv").textContent 
        }
    }
    xhttp.open("GET", "/allfiles.txt", true);
    xhttp.send();
}

getFilesFromGitHub();

function run() {

    //getMarkdownContent("/wiki/content/test.md");

    /*var text = doGET("/wiki/content/test.md"),
        target = document.getElementById('targetDiv'),
        converter = new showdown.Converter(),
        html = converter.makeHtml(text);
      
      target.innerHTML = html;*/
}