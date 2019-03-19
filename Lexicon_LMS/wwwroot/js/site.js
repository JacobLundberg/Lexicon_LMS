// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function ToggleDocs(id) {
    // var docBox = document.getElementById("Docs-" + id);
    var docIco = document.getElementById("Ico-" + id);

    //if (docBox.style.display == "none") {
    //    docBox.style.display = "block";
    //    docIco.classList.remove("fa-plus");
    //    docIco.classList.add("fa-minus");
    //}
    //else {
    //    docBox.style.display = "none";
    //    docIco.classList.remove("fa-minus");
    //    docIco.classList.add("fa-plus");
    //}

    if (docIco.classList.contains("fa-plus")) {
        docIco.classList.remove("fa-plus");
        docIco.classList.add("fa-minus");
    }
    else {
        docIco.classList.remove("fa-minus");
        docIco.classList.add("fa-plus");
    }

    return false;
}


