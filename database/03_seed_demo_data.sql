USE AISupportHub;
GO

INSERT INTO dbo.Conversations
(
    Title,
    SupportStatus,
    SupportName,
    SupportEmail,
    SupportDescription
)
VALUES
(
    'No puedo acceder al sistema',
    'TICKET_CREATED',
    'Usuario Demo',
    'demo@example.com',
    'El sistema indica que las credenciales son incorrectas.'
);

DECLARE @ConversationId INT =
    CAST(SCOPE_IDENTITY() AS INT);


INSERT INTO dbo.Messages
(
    ConversationId,
    Role,
    Content
)
VALUES
(
    @ConversationId,
    'user',
    'No puedo acceder al sistema.'
),
(
    @ConversationId,
    'assistant',
    'Entiendo. Voy a ayudarte a generar una solicitud de soporte.'
),
(
    @ConversationId,
    'user',
    'Usuario Demo'
),
(
    @ConversationId,
    'assistant',
    '¿Cuál es tu correo electrónico?'
),
(
    @ConversationId,
    'user',
    'demo@example.com'
),
(
    @ConversationId,
    'assistant',
    'Describe brevemente el problema que estás teniendo.'
),
(
    @ConversationId,
    'user',
    'El sistema indica que las credenciales son incorrectas.'
);
GO


DECLARE @ConversationId INT =
(
    SELECT TOP 1 Id
    FROM dbo.Conversations
    ORDER BY Id DESC
);

INSERT INTO dbo.Tickets
(
    ConversationId,
    TicketNumber,
    Name,
    Email,
    Description,
    Status
)
VALUES
(
    @ConversationId,
    'SUP-000001',
    'Usuario Demo',
    'demo@example.com',
    'El sistema indica que las credenciales son incorrectas.',
    'OPEN'
);
GO