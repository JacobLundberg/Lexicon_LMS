// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function toggleDocs(id) {
    // obj.style.color = "purple";

    var docBox = document.getElementById("Docs-" + id);
    if (docBox.style.display == "none")
        docBox.style.display = "block";
    else
        docBox.style.display = "none";

    // Docs-@item.Id

    return false;
}


