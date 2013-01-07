function shToggle(elem, blkid)
{
    var blk = document.getElementById("blk" + blkid);
    var expanded = (blk.style.display!="none");
    blk.style.display = (expanded ? "none" : "inline");
    elem.className = (expanded ? "sh_collapsed" : "sh_expanded");
    return false;
}
