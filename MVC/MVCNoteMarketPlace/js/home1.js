$(function (){
        

    // show/hide nav on page load
    showHideNav();
    
    $(window).scroll(function(){
        
        // // show/hide nav on window's scroll
        showHideNav();
    })
    
    function showHideNav(){
        
        if( $(window).scrollTop() > 50 ){
            
            // Show White nav
            $('.wrapper').addClass("white-header");
            
            // Show Dark nav
            $(".logo-wrapper img").attr("src", "/img/home/logo.png");
            
            // Show Back To Top Button
//            $("#back-to-top").fadeIn();
            
        } else {
            
            // Hide White nav
            $('.wrapper').removeClass("white-header");
            
            // Show logo
            $(".logo-wrapper img").attr("src", "/img/home/top-logo.png");
            
            // Hide back to top button
//            $("#back-to-top").fadeOut();
        }
        
    }
});
