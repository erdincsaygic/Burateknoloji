var myButtons = document.querySelectorAll('.bankbuttons');
myButtons.forEach(function(button) {
  button.addEventListener('click', function() {
    myButtons.forEach(function(btn) {
      btn.classList.remove('hoveractive');
    });
    this.classList.add('hoveractive');
  });
});

