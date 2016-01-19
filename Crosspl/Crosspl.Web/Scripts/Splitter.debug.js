function applyClientSideSplitters() {
    var resource = $("#splitResources");
    if (resource.length > 0) {
        resource.find(".splitResourceItem").each(function (index, divItem) {
            var sel = $(divItem).find(".splitResourceSelector:first").text();
            var val = $(divItem).find(".splitResourceFeed:first").text();
            $(sel).each(function (itemIndex, targetItem) {
                $(targetItem).html(val);
            });
        });
    }
}