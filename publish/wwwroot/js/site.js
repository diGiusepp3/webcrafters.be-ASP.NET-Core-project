document.addEventListener("DOMContentLoaded", function () {
    const unlockBtn = document.getElementById("unlockDomainCheck");
    const wrapper = document.getElementById("domain-check-wrapper");
    const form = document.getElementById("domain-check-form");
    const resultBox = document.getElementById("availability-message");

    // Toon/verberg het formulier bij klik op de knop
    if (unlockBtn && wrapper) {
        unlockBtn.addEventListener("click", function (e) {
            e.preventDefault();
            wrapper.style.display = "block";
            unlockBtn.style.display = "none"; // knop verbergen na klik
        });
    } 

    // AJAX submit
    if (form && resultBox) {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();
            const domain = document.getElementById("domain-input").value;

            const response = await fetch("/Domain/Check", {
                method: "POST",
                headers: { "Content-Type": "application/x-www-form-urlencoded" },
                body: `domain=${encodeURIComponent(domain)}`
            });

            const text = await response.text(); // altijd eerst text
            let data;

            try {
                data = JSON.parse(text);
            } catch (err) {
                console.error("Geen geldige JSON:", text);
                resultBox.innerHTML = "⚠️ Server gaf geen JSON terug.";
                resultBox.className = "mt-3 alert alert-warning";
                return;
            }

            resultBox.innerHTML = data.message;
            resultBox.className = "mt-3 alert " + (data.available ? "alert-success" : "alert-danger");
        });

    }
});


