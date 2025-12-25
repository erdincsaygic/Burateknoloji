// ************** STEP BY STEP START ****************
const progress = document.getElementById("progress");
const next = document.getElementById("next");
const back = document.getElementById("backs");
const circles = document.querySelectorAll(".circle");
const back2t = document.getElementById("back2t");
const buttonfinals = document.getElementById("step3g");

let currentActive = 1;

document.querySelectorAll(".hovers").forEach(function (link) {
    link.addEventListener("click", function (event) {
        //event.preventDefault();
        currentActive++;
        update();
        console.log(currentActive);
    });

});

document.querySelectorAll(".hoversback").forEach(function (link) {
    link.addEventListener("click", function (event) {
        //event.preventDefault();
        currentActive--;
        update();
        console.log(currentActive);
    });
});

/**back.addEventListener("click", ()=>{
    currentActive=1;
    update();
});

back2t.addEventListener("click", ()=>{
  currentActive=1;
  update();
});

buttonfinals.addEventListener("click", ()=>{
  currentActive=3;
  update();
});

document.getElementById("step3").addEventListener("click", function(event) {
    currentActive= 3;
    update();
  });**/


function update() {
    circles.forEach((circle, idx) => {
        if (idx < currentActive) {
            circle.classList.add("active");
        }
        else {
            circle.classList.remove("active");
        }
    });

    const actives = document.querySelectorAll(".active");
    progress.style.width =
        ((actives.length - 1) / (circles.length - 1)) * 100 + "%";
};
// ************** STEP BY STEP END ****************

// ************** ANİMASYON KODLARI START ****************
var popup = document.getElementById('fullscreen-popup');
const myButton = document.querySelector('#next');
const buttonfinal = document.getElementById("step3");
const button4 = document.getElementById("step4");
const button5 = document.getElementById("step5");
const button6 = document.getElementById("step6");
const stepefttellonay = document.getElementById("eftstellonay");
const stepeftkayit = document.getElementById("eftskayit");
const buttoncc = document.getElementById("nexts");
const stepeftonay = document.getElementById("eftsonay");
const myDiv = document.querySelector('.deneme');
const step1 = document.getElementById("step1");
const step2eft = document.getElementById("efts");
const step2cc = document.getElementById("step2cc");
const stepfinal = document.getElementById("stepfinal");
const backla = document.getElementById("backs");
const backla2 = document.getElementById("backs2");
const backst = document.getElementById("backst");
const backst2 = document.getElementById("backst2");

//myButton.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            step1.classList.add('disabled');
//            step2eft.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});

//buttoncc.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            step1.classList.add('disabled');
//            step2cc.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});

//buttonfinal.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {
//        setTimeout(function () {
//            step2eft.classList.add('disabled');
//            stepeftonay.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);
//    }
//});

//buttonfinals.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            step2cc.classList.add('disabled');
//            stepefttellonay.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});

//button4.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            stepeftonay.classList.add('disabled');
//            stepeftkayit.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});
//button5.addEventListener('click', () => {
//    timeback()
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            stepeftkayit.classList.add('disabled');
//            stepefttellonay.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});

//button6.addEventListener('click', () => {

//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            stepefttellonay.classList.add('disabled');
//            stepfinal.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);
//        currentActive = 5;
//        update();
//    }
//});


//backla.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            step2eft.classList.add('disabled');
//            step2cc.classList.add('disabled');
//            step1.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});

//backla2.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            stepeftonay.classList.add('disabled');
//            step2eft.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});

//backst.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            stepeftkayit.classList.add('disabled');
//            stepeftonay.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});
//backst2.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            stepefttellonay.classList.add('disabled');
//            stepeftkayit.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }
//});

//back2t.addEventListener('click', () => {
//    myDiv.style.maxHeight = '0rem';
//    myDiv.style.transition = 'max-height 0.5s ease-in-out';
//    if (myDiv.style.maxHeight = '0rem') {

//        setTimeout(function () {
//            step2eft.classList.add('disabled');
//            step2cc.classList.add('disabled');
//            step1.classList.remove('disabled');
//            myDiv.style.maxHeight = '200rem';
//        }, 500);

//    }


//});
// ************** ANİMASYON KODLARI END ****************


// ************** AKTİF BUTTON YAPISI START ****************

var myButtons = document.querySelectorAll('.bankbuttons');
myButtons.forEach(function (button) {
    button.addEventListener('click', function () {
        myButtons.forEach(function (btn) {
            btn.classList.remove('hoveractive');
        });
        this.classList.add('hoveractive');
    });
});

// ************** AKTİF BUTTON YAPISI END ****************
