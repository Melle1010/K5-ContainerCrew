const inputField = document.querySelector(".chat-input-field");
const sendBtn = document.querySelector(".chat-send-btn");
const chatHistory = document.querySelector(".chat-history");

const API_URL = "https://aicontent-app.ashyflower-20b74b17.swedencentral.azurecontainerapps.io/api/AiContent/generate/ai/posts";

sendBtn.addEventListener("click", sendMessage);
inputField.addEventListener("keydown", e => {
    if (e.key === "Enter") sendMessage();
});

function appendMessage(text, sender) {
    const wrapper = document.createElement("div");
    wrapper.classList.add("message", sender);

    const bubble = document.createElement("div");
    bubble.classList.add(sender === "bob" ? "bob-message" : "user-message");
    bubble.textContent = text;

    wrapper.appendChild(bubble);
    chatHistory.appendChild(wrapper);
    chatHistory.scrollTop = chatHistory.scrollHeight;
}

async function sendMessage() {
    const userText = inputField.value.trim();
    if (!userText) return;

    appendMessage(userText, "user");
    inputField.value = "";

    try {
        const response = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(userText)
        });

        if (!response.ok) {
            const errorText = `Error: ${response.status}`;
            appendMessage(errorText, "bob");
            return;
        }

        const data = await response.json();
        appendMessage(data.answer, "bob");

    } catch (err) {
        appendMessage("Bob is confused... something broke.", "bob");
    }
}
