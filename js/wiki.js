/*function xhttpSuccess(xhttp) {
    return xhttp.readyState == 4 && xhttp.status == 200;
}

function getFilesFromGitHub() {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function() {
        if(xhttpSuccess(xhttp)) {
            let latestCommit = JSON.parse(this.responseText);
            console.log(latestCommit);
        }
    }
    xhttp.open("GET", REPO_API + "commits/website", true);
    xhttp.send();
}

function run() {

    getFilesFromGitHub();
    //getMarkdownContent("/wiki/content/test.md");

    /*var text = doGET("/wiki/content/test.md"),
        target = document.getElementById('targetDiv'),
        converter = new showdown.Converter(),
        html = converter.makeHtml(text);
      
      target.innerHTML = html;
}*/