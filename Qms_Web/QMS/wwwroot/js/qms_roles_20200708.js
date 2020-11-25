let roleCodeCreate = document.getElementById("roleCodeCreate");
if (roleCodeCreate) {
    roleCodeCreate.addEventListener("input", function (event) {
        let validValues = '';
        for (let ii = 0; ii < event.target.value.length; ii++) {
            let testChar = event.target.value.charAt(ii);
            if (isAlpha(testChar)) {
                validValues += testChar.toUpperCase();
            }
            if (isDigit(testChar)) {
                validValues += testChar;
            }
            if (testChar === '_') {
                validValues += testChar;
            }
            if (testChar === ' ') {
                validValues += '_';
            }
        }
        roleCodeCreate.value = validValues;
    });
}

/**********************************************************************************************
let createRoleForm = document.getElementById("createRoleForm");
if (createRoleForm) {
    createRoleForm.addEventListener('submit', function (event) {
        if (createRoleForm.checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }


        createRoleForm.classList.add('was-validated');

        //var checkboxes = document.querySelectorAll("#createRoleForm input[type=checkbox]");
        for (var ii = 0; ii < createRoleForm.elements.length; ii++) {
            if (createRoleForm.elements[ii].type == 'checkbox') {
                console.log(createRoleForm.elements[ii]);
                //createRoleForm.elements[ii].setAttribute("class", "custom-control-input");
                createRoleForm.elements[ii].classList.remove("custom-control-input:valid");
                createRoleForm.elements[ii].style.color = "pink";

            }
        }
    }, false);
}
***********************************************************************************************/