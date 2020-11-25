function processPermssionUpdate(permissionUpdateForm, sourceValueId, targetValueId) {
    let sourceValObj = document.getElementById(sourceValueId);
    let targetValObj = document.getElementById(targetValueId);


    console.log(permissionUpdateForm);
    console.log("");
    console.log(`[BEFORE] sourceValObj.value: "${sourceValObj.value}"`);
    console.log(`[BEFORE] targetValObj.value: "${targetValObj.value}"`);

    targetValObj.value = sourceValObj.value;

    console.log(`[AFTER] sourceValObj.value: "${sourceValObj.value}"`);
    console.log(`[AFTER] targetValObj.value: "${targetValObj.value}"`);

    //return false;
    permissionUpdateForm.submit();
}

let permissionCodeCreate = document.getElementById("permissionCodeCreate");
if (permissionCodeCreate) {
    permissionCodeCreate.addEventListener("input", function (event) {
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
        permissionCodeCreate.value = validValues;
    });
}

let createPermissionForm = document.getElementById("createPermissionForm");
if (createPermissionForm) {
    createPermissionForm.addEventListener('submit', function (event) {
        if (createPermissionForm.checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }
        createPermissionForm.classList.add('was-validated');
    }, false);
}




























