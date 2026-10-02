let currentPage = 1;

let currentSortColumn = "";
let currentSortOrder = "asc";


// =====================================================
// PAGE LOAD
// =====================================================

document.addEventListener("DOMContentLoaded", function () {

    const table = document.getElementById("employeeTable");

    if (!table) {
        return;
    }

    // Load first page
    loadEmployees(1);


    // Search form
    const searchForm =
        document.getElementById("employeeSearchForm");

    if (searchForm) {

        searchForm.addEventListener("submit", function (event) {

            event.preventDefault();

            loadEmployees(1);

        });

    }


    // Clear filters
    const clearButton =
        document.getElementById("clearFilters");

    if (clearButton) {

        clearButton.addEventListener("click", function () {

            document.getElementById("searchInput").value = "";

            document.getElementById("departmentFilter").value = "";

            document.getElementById("statusFilter").value = "";

            currentSortColumn = "";
            currentSortOrder = "asc";

            resetSortButtons();

            loadEmployees(1);

        });

    }


    // Sorting buttons
    const sortButtons =
        table.querySelectorAll(".sort-btn");


    sortButtons.forEach(button => {

        button.addEventListener("click", function () {

            const column =
                this.dataset.column;

            // Same column → toggle order
            if (currentSortColumn === column) {

                currentSortOrder =
                    currentSortOrder === "asc"
                        ? "desc"
                        : "asc";

            }
            else {

                // New column
                currentSortColumn = column;

                currentSortOrder = "asc";

            }


            updateSortIcons(this);

            // Sorting should start from page 1
            loadEmployees(1);

        });

    });

});


// =====================================================
// LOAD EMPLOYEES
// =====================================================

async function loadEmployees(pageNumber = 1) {

    currentPage = pageNumber;

    showLoading();


    try {

        // Get search/filter values
        const search =
            document.getElementById("searchInput")?.value || "";

        const department =
            document.getElementById("departmentFilter")?.value || "";

        const status =
            document.getElementById("statusFilter")?.value || "";


        // Build query string
        const params = new URLSearchParams();

        params.append("pageNumber", pageNumber);


        if (search.trim() !== "") {

            params.append(
                "search",
                search.trim()
            );

        }


        if (department !== "") {

            params.append(
                "department",
                department
            );

        }


        if (status !== "") {

            params.append(
                "status",
                status
            );

        }


        if (currentSortColumn !== "") {

            params.append(
                "sortColumn",
                currentSortColumn
            );

            params.append(
                "sortOrder",
                currentSortOrder
            );

        }


        // AJAX request
        const response = await fetch(
            `/Employee/GetEmployees?${params.toString()}`
        );


        if (!response.ok) {

            throw new Error(
                "Failed to load employees."
            );

        }


        const data =
            await response.json();


        // Render employee records
        renderEmployees(data.items);


        // Render employee count
        renderEmployeeCount(
            data.totalRecords
        );


        // Render pagination
        renderPagination(
            data.currentPage,
            data.totalPages
        );

    }
    catch (error) {

        console.error(
            "Error loading employees:",
            error
        );

    }
    finally {

        hideLoading();

    }

}


// =====================================================
// RENDER EMPLOYEES
// =====================================================

function renderEmployees(employees) {

    const tbody =
        document.getElementById(
            "employeeTableBody"
        );


    const noEmployeeMessage =
        document.getElementById(
            "noEmployeeMessage"
        );


    if (!tbody) {
        return;
    }


    tbody.innerHTML = "";


    // No employees
    if (!employees || employees.length === 0) {

        if (noEmployeeMessage) {

            noEmployeeMessage.classList.remove(
                "d-none"
            );

        }

        return;

    }


    if (noEmployeeMessage) {

        noEmployeeMessage.classList.add(
            "d-none"
        );

    }


    employees.forEach(employee => {

        const row = `

            <tr>

                <!-- Employee Code -->

                <td class="px-3 fw-semibold">

                    ${employee.employeeCode}

                </td>


                <!-- Employee Name -->

                <td>

                    <div class="fw-semibold">

                        ${employee.firstName}
                        ${employee.lastName}

                    </div>

                </td>


                <!-- Email -->

                <td>

                    <span class="text-muted">

                        ${employee.email}

                    </span>

                </td>


                <!-- Phone -->

                <td>

                    ${employee.phone}

                </td>


                <!-- Department -->

                <td>

                    <span class="badge bg-light text-dark border">

                        ${employee.department}

                    </span>

                </td>


                <!-- Designation -->

                <td>

                    ${employee.designation}

                </td>


                <!-- Salary -->

                <td class="fw-semibold">

                    ₹${Number(employee.salary).toLocaleString(
            "en-IN",
            {
                minimumFractionDigits: 2
            }
        )}

                </td>


                <!-- Status -->

                <td>

                    ${employee.status === "Active"

                ? `
                            <span class="badge bg-success">
                                Active
                            </span>
                          `

                : `
                            <span class="badge bg-danger">
                                ${employee.status}
                            </span>
                          `
            }

                </td>


                <!-- Image -->

                <td>

                    ${employee.imagePath

                ? `
                            <img
                                src="${employee.imagePath}"
                                alt="${employee.firstName}"
                                width="50"
                                height="50"
                                class="rounded-circle employee-image">
                          `

                : `
                            <div class="default-avatar">

                                ${employee.firstName
                    .substring(0, 1)
                    .toUpperCase()}

                            </div>
                          `
            }

                </td>


                <!-- Actions -->

                <td class="text-center">

                    <div class="btn-group">

                        <!-- Details -->

                        <a href="/Employee/Details/${employee.employeeId}"
                           class="btn btn-sm btn-info text-white"
                           title="View Details">

                            <i class="bi bi-eye"></i>

                        </a>


                        <!-- Edit -->

                        <a href="/Employee/Edit/${employee.employeeId}"
                           class="btn btn-sm btn-warning"
                           title="Edit Employee">

                            <i class="bi bi-pencil"></i>

                        </a>


                        <!-- Delete -->

                        <a href="/Employee/Delete/${employee.employeeId}"
                           class="btn btn-sm btn-danger"
                           title="Delete Employee">

                            <i class="bi bi-trash"></i>

                        </a>

                    </div>

                </td>

            </tr>

        `;


        tbody.insertAdjacentHTML(
            "beforeend",
            row
        );

    });

}


// =====================================================
// EMPLOYEE COUNT
// =====================================================

function renderEmployeeCount(totalRecords) {

    const employeeCount =
        document.getElementById(
            "employeeCount"
        );


    if (!employeeCount) {
        return;
    }


    employeeCount.textContent =
        `${totalRecords} Employees`;

}


// =====================================================
// PAGINATION
// =====================================================

function renderPagination(
    currentPage,
    totalPages
) {

    const container =
        document.getElementById(
            "paginationContainer"
        );


    if (!container) {
        return;
    }


    container.innerHTML = "";


    if (totalPages <= 1) {
        return;
    }


    let html = `

        <nav>

            <ul class="pagination justify-content-center">

    `;


    // =================================================
    // PREVIOUS
    // =================================================

    html += `

        <li class="page-item
            ${currentPage === 1 ? "disabled" : ""}">

            <button
                type="button"
                class="page-link"
                onclick="loadEmployees(${currentPage - 1})"
                ${currentPage === 1 ? "disabled" : ""}>

                Previous

            </button>

        </li>

    `;


    // =================================================
    // PAGE NUMBERS
    // =================================================

    for (
        let i = 1;
        i <= totalPages;
        i++
    ) {

        html += `

            <li class="page-item
                ${i === currentPage ? "active" : ""}">

                <button
                    type="button"
                    class="page-link"
                    onclick="loadEmployees(${i})">

                    ${i}

                </button>

            </li>

        `;

    }


    // =================================================
    // NEXT
    // =================================================

    html += `

        <li class="page-item
            ${currentPage === totalPages ? "disabled" : ""}">

            <button
                type="button"
                class="page-link"
                onclick="loadEmployees(${currentPage + 1})"
                ${currentPage === totalPages ? "disabled" : ""}>

                Next

            </button>

        </li>

    `;


    html += `

            </ul>

        </nav>

    `;


    container.innerHTML = html;

}


// =====================================================
// SORT ICON
// =====================================================

function updateSortIcons(activeButton) {

    const table =
        document.getElementById(
            "employeeTable"
        );


    const sortButtons =
        table.querySelectorAll(
            ".sort-btn"
        );


    sortButtons.forEach(button => {

        const icon =
            button.querySelector("span");


        button.dataset.order = "none";


        if (icon) {

            icon.textContent = "↕";

        }

    });


    // Active button
    activeButton.dataset.order =
        currentSortOrder;


    const activeIcon =
        activeButton.querySelector("span");


    if (activeIcon) {

        activeIcon.textContent =
            currentSortOrder === "asc"
                ? "↑"
                : "↓";

    }

}


// =====================================================
// RESET SORT
// =====================================================

function resetSortButtons() {

    const table =
        document.getElementById(
            "employeeTable"
        );


    if (!table) {
        return;
    }


    const sortButtons =
        table.querySelectorAll(
            ".sort-btn"
        );


    sortButtons.forEach(button => {

        button.dataset.order = "none";


        const icon =
            button.querySelector("span");


        if (icon) {

            icon.textContent = "↕";

        }

    });

}


// =====================================================
// LOADING
// =====================================================

function showLoading() {

    const loading =
        document.getElementById(
            "loadingIndicator"
        );


    if (loading) {

        loading.classList.remove(
            "d-none"
        );

    }

}


function hideLoading() {

    const loading =
        document.getElementById(
            "loadingIndicator"
        );


    if (loading) {

        loading.classList.add(
            "d-none"
        );

    }

}