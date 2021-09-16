$(document).ready(function() {	
	
	$(document).mouseup(function (e) { 
		var container = $(".showDD");
		var container1 = $(".ddPopup");
		if (!container.is(e.target)  && !container1.is(e.target)   && container.has(e.target).length === 0) // ... nor a descendant of the container
			{	
			   container1.stop().slideUp(300);
			  $(".ddPopupCov").removeClass("active");
			} 
		});		
	$(".showDD").click(function(){
		$(".ddPopup").slideUp(300);	
		if($(this).parent(".ddPopupCov").hasClass("active")){
				$(".ddPopupCov").removeClass("active");
				$(this).next(".ddPopup").slideUp(300);
		}
		else{
				$(".ddPopupCov").removeClass("active");
				$(this).parent(".ddPopupCov").addClass("active");
				$(this).next(".ddPopup").slideDown(300);
		}
	});

	
	
	$(".loginBtn").on('click', function (e) {

		e.preventDefault();
		var popId=$(this).attr("href");
		$(popId).fadeIn(300);
		$("body").css("overflow", "hidden")
		$(".overlay").fadeIn(300);
		var  popupheight = $(popId).children(".popupBox").height()/2;
		//$(popId).children(".popupBox").css("marginTop", (-popupheight));
		$(".signInErrorDiv").hide();
	});	
	$(".closePopup").click(function() {
		$(".overlay").fadeOut(300);
		$("body").css("overflow","auto")
	});	
});

$(window).resize(function() {
	//var  popupheight = $(".popupBox").height()/2;
	//$(".popupBox").css("marginTop",(-popupheight));
});
