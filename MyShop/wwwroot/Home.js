
const meter = document.querySelector("#a")
const registerContainer = document.querySelector("#registerContainer");
const loginContainer = document.querySelector("#loginContainer");
let flag = false;

const register = () => {
    loginContainer.style.visibility = "hidden";
    registerContainer.style.visibility = "visible";
}

const getDataFromRegister = () => {
    const userName = document.querySelector("#registerUserNameInput").value
    const password = document.querySelector("#registerPasswordInput").value
    const firstName = document.querySelector("#registerFirstNameInput").value
    const lastName = document.querySelector("#registerLastNameInput").value

    return ({ userName, password, firstName, lastName })
}
const createNewUser = async () => {
    if (!flag) {
        alert("Enter password again")
    }
    else {
        const newUser = getDataFromRegister();
        try {
            const responsePost = await fetch('api/User', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(newUser)
            });
            const responseData = await responsePost.json();

            if (responsePost.ok) {
                alert(`user ${responseData.userId} created successfully` );
            }
            else {
                if (responsePost.status === 409) {
                    alert("User name already exists");
                }
                if (responseData.errors) {
                    const errorMessages = Object.entries(responseData.errors)
                        .map(([field, messages]) => `${field}: ${messages.join(", ")}`)
                        .join("\n");
                    alert(errorMessages);
                } else {
                    alert("One or more details is wrong");
                }
            }  
            
        }
        catch (error) {
            console.error(error)
        }
    }
}
const getDataFromLogin = () => {
    const userName = document.querySelector("#loginUserNameInput").value
    const password = document.querySelector("#loginPasswordInput").value
    return ({ userName, password })
}




const login = async () => {
    const details = getDataFromLogin();
    try {
        const responsePost = await fetch(`api/User/Login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(details)
        });
        const dataPost = await responsePost.json();
        if (dataPost.status == 400)
            alert("wrong details, please try again!")
        else {
            localStorage.setItem("currentUser", JSON.stringify(dataPost))
            window.location.href = "UserDetails.html";
        }
    }
    catch (error) {
console.error(error)    }
}

const getPassword = () => {
    const password = document.querySelector("#registerPasswordInput").value
    return ( password )
}

const checkPassword = async () => {
    const password = getPassword()
    try {
        const responsePost = await fetch(`api/User/Password`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(password)
        });
        if (!responsePost.ok) {
            alert("Enter password")
        }
        else {
            const dataPost = await responsePost.json();
            if (dataPost<2) {
                flag = false;
            }
            else {
                flag = true;
            }
            meter.value = (dataPost / 10) * 2 + 0.2
            return dataPost
        }
    }
    catch (error) {
        console.error(error)
    }
}