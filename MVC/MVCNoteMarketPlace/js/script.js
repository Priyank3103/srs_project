$(function(){
    
    // Show mobile nav
    $("#mobile-nav-open-btn").click(function(){
        $("#mobile-nav").css("height", "100%");
    })
    // Hide mobile nav
    $("#mobile-nav-close-btn, #mobile-nav a").click(function(){
        $("#mobile-nav").css("height", "0%");
    })
});

$(function(){
    $("#download").click(function(){
        $("#download-content").css("display", "block");
//        window.alert("hii")
    })
    
//    $(".table-responsive-lg, .table #close-tab").click(function(){
//        $("#download-content").hide();
//    })
    
});
