$(() => {  
    $("#shorten").on('click', function () {    
        const originalURL = $("#URL").val();         
        $.post('/home/shortenurl', { originalURL }, function (newURL) { 
            $("#new-url").remove();
            $(".well").append(` <h3 class="text-center" id="new-url">New URL: <a target="_blank" href="${newURL}" >${newURL}</a></h3>`)          
        });     
    });  
});