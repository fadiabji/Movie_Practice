
// give a red circule around around the amount number that cart has.
const element = document.getElementById("shopcount");
console.log(element.Value)
// Check if the element has a value
if (element) {
    // Add a style to the element
    element.style.backgroundColor = "red";
    element.style.textAlign = "center";
    element.style.color = "#fff";
    element.style.width = "20px";
    element.style.hight = "25px";
    element.style.borderRadius = "50%";
    element.style.fontSize = "14px";
}

// check out form

(function () {
    'use strict'
    const forms = document.querySelectorAll('.requires-validation')
    Array.from(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault()
                    event.stopPropagation()
                }

                form.classList.add('was-validated')
            }, false)
        })
})()
