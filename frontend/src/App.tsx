import { useEffect, useState } from "react";
import "./App.css";
import Dashboard from "./pages/Dashboard";

interface Message {
  text: string;
  sender: "user" | "bot";
}

interface Conversation {
  id: number;
  title: string;
  createdAt: string;
}

interface ApiMessage {
  id: number;
  conversationId: number;
  role: string;
  content: string;
  createdAt: string;
}

function App() {
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);
  const [loading, setLoading] = useState(false);
  const [currentPage, setCurrentPage] =
  useState<"chat" | "dashboard" | "tickets" | "documents">(
    "chat"
  );

  const [conversationId, setConversationId] =
    useState<number | null>(null);

  const [conversations, setConversations] =
    useState<Conversation[]>([]);

  const apiUrl = "https://localhost:7156";

  const loadConversations = async () => {
    try {
      const response = await fetch(
        `${apiUrl}/api/conversations`
      );

      if (!response.ok) {
        throw new Error(
          "No fue posible cargar las conversaciones."
        );
      }

      const data: Conversation[] =
        await response.json();

      setConversations(data);
    } catch (error) {
      console.error(
        "Error cargando conversaciones:",
        error
      );
    }
  };

  const openConversation = async (id: number) => {
    try {
      const response = await fetch(
        `${apiUrl}/api/conversations/${id}/messages`
      );

      if (!response.ok) {
        throw new Error(
          "No fue posible cargar la conversación."
        );
      }

      const data: ApiMessage[] =
        await response.json();

      setConversationId(id);

      setMessages(
        data.map((item) => ({
          text: item.content,
          sender:
            item.role === "assistant"
              ? "bot"
              : "user",
        }))
      );
    } catch (error) {
      console.error(
        "Error cargando conversación:",
        error
      );
    }
  };

  const newConversation = () => {
    setConversationId(null);
    setMessages([]);
    setMessage("");
  };

  useEffect(() => {
    loadConversations();
  }, []);

  const sendMessage = async () => {
    if (!message.trim() || loading) {
      return;
    }

    const userMessage = message.trim();

    setMessages((prev) => [
      ...prev,
      {
        text: userMessage,
        sender: "user",
      },
    ]);

    setMessage("");
    setLoading(true);

    try {
      const response = await fetch(
        `${apiUrl}/api/chat`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            message: userMessage,
            conversationId: conversationId,
          }),
        }
      );

      if (!response.ok) {
        const errorText = await response.text();

        throw new Error(
          `Error del servidor: ${response.status} ${errorText}`
        );
      }

      const data = await response.json();

      setConversationId(data.conversationId);

      setMessages((prev) => [
        ...prev,
        {
          text: data.message,
          sender: "bot",
        },
      ]);

      await loadConversations();
    } catch (error) {
      console.error(
        "Error enviando mensaje:",
        error
      );

      setMessages((prev) => [
        ...prev,
        {
          text:
            "No fue posible conectarse con el servidor.",
          sender: "bot",
        },
      ]);
    } finally {
      setLoading(false);
    }
  };
  const renameConversation = async (
  id: number,
  currentTitle: string
) => {
  const newTitle = window.prompt(
    "Nuevo nombre de la conversación:",
    currentTitle
  );

  if (!newTitle?.trim()) {
    return;
  }

  try {
    const response = await fetch(
      `${apiUrl}/api/conversations/${id}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          title: newTitle.trim(),
        }),
      }
    );

    if (!response.ok) {
      throw new Error(
        "No fue posible renombrar la conversación."
      );
    }

    await loadConversations();
  } catch (error) {
    console.error(
      "Error renombrando conversación:",
      error
    );
  }
};
const deleteConversation = async (id: number) => {
  const confirmed = window.confirm(
    "¿Seguro que deseas eliminar esta conversación?"
  );

  if (!confirmed) {
    return;
  }

  try {
    const response = await fetch(
      `${apiUrl}/api/conversations/${id}`,
      {
        method: "DELETE",
      }
    );

    if (!response.ok) {
      throw new Error(
        "No fue posible eliminar la conversación."
      );
    }

    if (conversationId === id) {
      newConversation();
    }

    await loadConversations();
  } catch (error) {
    console.error(
      "Error eliminando conversación:",
      error
    );
  }
};

  return (
    <div className="app">
      <aside className="sidebar">

  <div className="sidebar-header">
    <h2>AI Support Hub</h2>
  </div>

  <div className="main-navigation">

    <button
      className={`nav-item ${
        currentPage === "dashboard"
          ? "active"
          : ""
      }`}
      onClick={() =>
        setCurrentPage("dashboard")
      }
    >
      📊 Dashboard
    </button>

    <button
      className={`nav-item ${
        currentPage === "chat"
          ? "active"
          : ""
      }`}
      onClick={() =>
        setCurrentPage("chat")
      }
    >
      💬 Chat
    </button>

    <button
      className={`nav-item ${
        currentPage === "tickets"
          ? "active"
          : ""
      }`}
      onClick={() =>
        setCurrentPage("tickets")
      }
    >
      🎫 Tickets
    </button>

    <button
      className={`nav-item ${
        currentPage === "documents"
          ? "active"
          : ""
      }`}
      onClick={() =>
        setCurrentPage("documents")
      }
    >
      📄 Documentos
    </button>

  </div>

  {currentPage === "chat" && (
    <>
      <button
        className="new-chat"
        onClick={newConversation}
      >
        + Nueva conversación
      </button>

      <div className="conversation-list">
    {conversations.map((conversation) => (
        <div key={conversation.id} className="conversation-row">
          <button
            className={`conversation-item ${
              conversationId === conversation.id
                ? "active"
                : ""
            }`}
            onClick={() =>
              openConversation(conversation.id)
            }
          >
            {conversation.title}
          </button>

          <button
            className="conversation-action"
            title="Renombrar conversación"
            onClick={() =>
              renameConversation(
                conversation.id,
                conversation.title
              )
            }
          >
            ✎
          </button>

          <button
            className="conversation-action delete"
            title="Eliminar conversación"
            onClick={() =>
              deleteConversation(
                conversation.id
              )
            }
          >
            ×
          </button>
        </div>
      ))}
    </div>
    </>
  )}

</aside>

    {currentPage === "dashboard" && (
      <Dashboard apiUrl={apiUrl} />
    )}

    {currentPage === "chat" && (
    <div className="chat-container">
            <header>
              <div>
                <h1>AI Support Hub</h1>
                <p>
                  Asistente inteligente empresarial
                </p>
              </div>
            </header>

            <main className="messages">
              {messages.length === 0 && (
                <div className="welcome">
                  <h2>👋 Hola</h2>

                  <p>
                    Soy tu asistente virtual.
                    ¿En qué puedo ayudarte?
                  </p>
                </div>
              )}

              {messages.map((item, index) => (
                <div
                  key={index}
                  className={`message ${item.sender}`}
                >
                  {item.text}
                </div>
              ))}

              {loading && (
                <div className="message bot">
                  Pensando...
                </div>
              )}
            </main>

            <footer>
              <input
                type="text"
                placeholder="Escribe tu consulta..."
                value={message}
                disabled={loading}
                onChange={(e) =>
                  setMessage(e.target.value)
                }
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    sendMessage();
                  }
                }}
              />

              <button
                onClick={sendMessage}
                disabled={
                  loading || !message.trim()
                }
              >
                {loading
                  ? "Enviando..."
                  : "Enviar"}
              </button>
            </footer>
          </div>
    )}
      
    </div>
  );
}

export default App;