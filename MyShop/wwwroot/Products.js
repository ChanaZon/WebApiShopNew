const templateCard = document.querySelector('#temp-card'); 
const templateCategory = document.querySelector('#temp-category');
const productList = document.querySelector('#ProductList'); 
const categoryList = document.querySelector('#categoryList')
const itemsCount = document.querySelector('#ItemsCountText');
const user = JSON.parse(localStorage.getItem("currentUser"))
const counter = document.querySelector("#counter")

let products = []

document.addEventListener('DOMContentLoaded', async () => {

    const categories = await LoadCategories()
     products = await LoadProductsList("api/product")
    PrintProducts(products)
    counter.innerText = products.length

    if (categories && categories.length > 0) {
        categories.forEach(category => {
            addCategory(category.categoryName,category.categoryId);
        });
    }
    const cartItems = localStorage.getItem(`${user.userId}_CartItems`)
    if(cartItems)
        itemsCount.innerText = JSON.parse(cartItems).length
    else
        itemsCount.innerText = '0'
})
const goToUserDetails = () => {
    window.location.href = ("UserDetails.html")
}

const addProduct = (name, price, description, image) => {

    const clone = templateCard.content.cloneNode(true); 

    const imgElement = clone.querySelector('img');
    imgElement.src = `/pictures/${image}.jpg`;
    imgElement.alt = name;

    clone.querySelector('h1').textContent = name;
    clone.querySelector('.price').textContent = `$${price}`;
    clone.querySelector('.description').textContent = description;
    productList.appendChild(clone);
};
const addCategory = (name,count) => {
    if (!templateCategory || !templateCategory.content) {
        console.error("Template for categories is not found!");
        return;
    }

    const clone = templateCategory.content.cloneNode(true);

    clone.querySelector('.OptionName').textContent = name;
    clone.querySelector('.OptionName').name = count

    categoryList.appendChild(clone);
};
let url = "api/product"
let flag = false
const filterProducts =async () => {
    const nameSearch = document.querySelector("#nameSearch").value
    const minPrice = document.querySelector("#minPrice").value
    const maxPrice = document.querySelector("#maxPrice").value

    if (nameSearch || nameSearch != "") {
        if (!flag) {
            url += `?desc=${nameSearch}`
            flag = true
        }
        else {
            url += `&desc=${nameSearch}`
        }
    }
    if (minPrice || minPrice != "") {
        if (!flag) {
            url += `?minPrice=${minPrice}`
            flag = true
        }
        else {
            url += `&minPrice=${minPrice}`
        }    }
    if (maxPrice || maxPrice != "") {
        if (!flag) {
            url += `?maxPrice=${maxPrice}`
            flag = true
        }
        else {
            url += `&maxPrice=${maxPrice}`
        }
}
    products = await LoadProductsList(url)
    PrintProducts()
    counter.innerText = products.length
}
const ClearFilters = async () => {
     products = await LoadProductsList("api/product")
    PrintProducts()
}
const changeCategory = async(checkbox) => {
    const template = checkbox.closest('.cb');
    const categoryId = template.querySelector('.OptionName').name;
    if (checkbox.checked) {
        if (!flag) {
            url += `?categoryIds=${categoryId}`
            flag = true
        }
        else {
            url += `&categoryIds=${categoryId}`
        }
        products = await LoadProductsList(url)
        PrintProducts()
        counter.innerText = products.length
    }
}

const PrintProducts = () => {
    productList.innerHTML=""
    products.forEach(product => {
        addProduct(product.productName, product.price, product.description, product.imageUrl);
    });
}
const LoadProductsList = async (url) => {
    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            console.error('שגיאה בעת הבאת נתונים מה-API:', response.status);
            return [];
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('שגיאה בעת טעינת הנתונים מה-API:', error);
        return [];
    }
};
const LoadCategories = async () => {
    try {
        const response = await fetch("api/category", {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            console.error('שגיאה בעת הבאת נתונים מה-API:', response.status);
            return [];
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('שגיאה בעת טעינת הנתונים מה-API:', error);
        return [];
    }
};

const AddToCart = (product) => {
    const card = product.closest('.card');
    const productName = card.querySelector('.productName').innerText;
    const p = products.find(p => p.productName == productName)
    const items = JSON.parse(localStorage.getItem(`${user.userId}_CartItems`))||[] 
    items.push(p)
    updateProductCount(items.length)
    localStorage.setItem(`${user.userId}_CartItems`, JSON.stringify(items))
}

const updateProductCount=(num) => {
    itemsCount.innerText = num; 
}