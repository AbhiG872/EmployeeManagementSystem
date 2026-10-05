document.addEventListener("DOMContentLoaded", function () {

    const departmentDropdown =
        document.getElementById("DepartmentId");

    const designationDropdown =
        document.getElementById("DesignationId");


    if (!departmentDropdown || !designationDropdown) {
        return;
    }


    departmentDropdown.addEventListener("change", function () {

        const departmentId = this.value;


        // Clear designation dropdown
        designationDropdown.innerHTML =
            '<option value="">Select Designation</option>';


        if (!departmentId) {
            return;
        }


        fetch(`/Employee/GetDesignations?departmentId=${departmentId}`)
            .then(response => {

                if (!response.ok) {
                    throw new Error("Failed to load designations.");
                }

                return response.json();

            })
            .then(data => {

                data.forEach(item => {

                    const option =
                        document.createElement("option");

                    option.value = item.id;
                    option.textContent = item.name;

                    designationDropdown.appendChild(option);

                });

            })
            .catch(error => {

                console.error(
                    "Error loading designations:",
                    error
                );

            });

    });

});