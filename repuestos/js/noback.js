function nobackbutton(){
2	
   window.location.hash="no-back-button";
3	
   window.location.hash="Again-No-back-button" //chrome
4	
   window.onhashchange=function(){window.location.hash="no-back-button";}
5	
}