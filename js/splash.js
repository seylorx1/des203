let navItems = document.getElementsByClassName('splash__nav-item');
if(navItems !== null && navItems.length > 0) {
    for(i = 0; i < navItems.length; i++) {
        let navItem = navItems.item(i);

        if(navItem.children !== null && navItem.children.length > 0) {
            for(j = 0; j < navItem.children.length; j++) {
                let navItemChild = navItem.children.item(j);
                if(navItemChild !== null && navItemChild.classList.contains('splash__nav--icon')) {
                    navItem.addEventListener("mouseover", function() {
                        if(!navItemChild.classList.contains("onHover")) {
                            navItemChild.classList.add("onHover")
                        }
                    });
                    navItem.addEventListener("mouseout", function() {
                        if(navItemChild.classList.contains("onHover")) {
                            navItemChild.classList.remove("onHover")
                        }
                    });
                    if(navItem.hasAttribute("href")) {

                        navItem.style.cursor = "pointer";

                        navItem.addEventListener("click", function() {
                            window.open(navItem.getAttribute("href"), "_self");
                        });
                    }
                    break;
                }
            }
        }
    }
}