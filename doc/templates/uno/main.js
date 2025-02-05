// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.

document.addEventListener(
    "DOMContentLoaded",
    function () {

        const navbar = document.querySelector("header > .navbar");
        if (document.body.classList.contains("front-page")) {
            let last_known_scroll_position = 0;
            let ticking = false;

            function doSomething(scroll_pos) {
                if (scroll_pos >= 100) navbar.classList.add("scrolled");
                else navbar.classList.remove("scrolled");
            }

            window.addEventListener("scroll", function () {
                last_known_scroll_position = window.scrollY;

                if (!ticking) {
                    window.requestAnimationFrame(function () {
                        doSomething(last_known_scroll_position);
                        ticking = false;
                    });

                    ticking = true;
                }
            });
        }

        const unoMenuReq = new XMLHttpRequest();
        const unoMenuEndpoint = "https://platform.uno/wp-json/wp/v2/menu";
        const $navbar = document.getElementById("navbar");
        let wordpressMenuHasLoaded = false;

        unoMenuReq.open("get", unoMenuEndpoint, true);

        if (typeof navbar !== "undefined") {
            unoMenuReq.onload = function () {
                if (unoMenuReq.status === 200 && unoMenuReq.responseText) {
                    $navbar.innerHTML = JSON.parse(
                        unoMenuReq.responseText
                    );
                    wordpressMenuHasLoaded = true;
                    $(document).trigger("wordpressMenuHasLoaded");
                }
            };
            unoMenuReq.onerror = function (e) {};
            unoMenuReq.send();
        }

        $( document ).ajaxComplete(function( event, xhr, settings ) {
            const docFxNavbarHasLoaded = settings.url === "toc.html";

            if (docFxNavbarHasLoaded && wordpressMenuHasLoaded) {
                const $docfxNavbar = $navbar.getElementsByClassName("navbar-nav");
                $docfxNavbar[0].className += " hidden";

            }
        });

        document.addEventListener(
            "click",
            function (e) {
                const t = e.target;
                if (
                    window.innerWidth >= 980 ||
                    !t.matches("#navbar .has-children a")
                )
                    return;
                e.preventDefault();
                t.parentElement.classList.toggle("open");
            },
            false
        );
    },
    false
);
