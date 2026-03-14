window.onload = function () {

    const originalFetch = window.fetch;

    window.fetch = async function (...args) {

        const response = await originalFetch(...args);

        try {

            const url = args[0];

            if (url.includes("/api/Auth/login")) {

                const data = await response.clone().json();

                if (data.accessToken) {

                    const token = data.accessToken;

                    localStorage.setItem("jwt", token);

                    if (window.ui) {
                        window.ui.preauthorizeApiKey("Bearer", token);
                    }

                    console.log("JWT Auto Applied:", token);
                }
            }

        } catch (e) {
            console.log(e);
        }

        return response;
    };
};