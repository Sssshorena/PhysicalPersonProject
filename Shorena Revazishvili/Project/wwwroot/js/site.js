// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    particlesJS("particles-js", {
        "particles": {
            "number": { "value": 100, "density": { "enable": true, "value_area": 900 } }, 
            "color": { "value": "#ffffff" },
            "shape": {
                "type": "circle",
                "stroke": { "width": 0, "color": "#000000" },
                "polygon": { "nb_sides": 5 }
            },
            "opacity": { "value": 0.6, "random": false },
            "size": { "value": 3, "random": true },
            "line_linked": { "enable": true, "distance": 140, "color": "#ffffff", "opacity": 0.4, "width": 1 },
            "move": { "enable": true, "speed": 1.5, "direction": "none", "random": false, "straight": false, "out_mode": "out" }
        },
        "interactivity": {
            "detect_on": "window", 
            "events": {
                "onhover": { "enable": true, "mode": "repulse" }, 
                "onclick": { "enable": true, "mode": "push" } 
            },
            "modes": {
                "repulse": { "distance": 100, "duration": 0.4 },
                "push": { "particles_nb": 5 } 
            }
        },
        "retina_detect": true
    });

    console.log("Particles.js initialized successfully!");
});

// გასააქტიურებლად ვიყენებთ შემდეგ ბიბლიოთეკას:
// <script src="https://cdn.jsdelivr.net/particles.js/2.0.0/particles.min.js"></script>
// <div id="particles-js"></div>
