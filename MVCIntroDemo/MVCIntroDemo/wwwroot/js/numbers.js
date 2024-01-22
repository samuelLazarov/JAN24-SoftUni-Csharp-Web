function SetLimit() {
    let num = document.getElementById("limitInput").value || 50;

    window.location = "https://localhost:7092/Numbers/Limit?num=" + num;
}