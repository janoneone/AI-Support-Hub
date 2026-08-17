import { useEffect, useState } from "react";

interface DashboardSummary {
  totalTickets: number;
  openTickets: number;
  closedTickets: number;
  conversations: number;
}

interface Ticket {
  id: number;
  ticketNumber: string;
  name: string;
  email: string;
  description: string;
  status: string;
  createdAt: string;
}

interface DashboardProps {
  apiUrl: string;
}

function Dashboard({ apiUrl }: DashboardProps) {
  const [summary, setSummary] =
    useState<DashboardSummary | null>(null);

  const [tickets, setTickets] =
    useState<Ticket[]>([]);

  const [loading, setLoading] =
    useState(true);

  const loadDashboard = async () => {
    try {
      const [summaryResponse, ticketsResponse] =
        await Promise.all([
          fetch(`${apiUrl}/api/dashboard/summary`),
          fetch(`${apiUrl}/api/tickets`)
        ]);

      if (!summaryResponse.ok) {
        throw new Error(
          "Error cargando estadísticas"
        );
      }

      if (!ticketsResponse.ok) {
        throw new Error(
          "Error cargando tickets"
        );
      }

      const summaryData =
        await summaryResponse.json();

      const ticketsData =
        await ticketsResponse.json();

      setSummary(summaryData);
      setTickets(ticketsData);
    } catch (error) {
      console.error(
        "Error cargando dashboard:",
        error
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDashboard();
  }, []);

  if (loading) {
    return (
      <div className="dashboard-page">
        Cargando dashboard...
      </div>
    );
  }

  return (
    <div className="dashboard-page">
      <div className="dashboard-header">
        <div>
          <h1>Dashboard</h1>
          <p>
            Resumen general de AI Support Hub
          </p>
        </div>
      </div>

      <div className="dashboard-cards">
        <div className="dashboard-card">
          <span>Total tickets</span>
          <strong>
            {summary?.totalTickets ?? 0}
          </strong>
        </div>

        <div className="dashboard-card">
          <span>Tickets abiertos</span>
          <strong>
            {summary?.openTickets ?? 0}
          </strong>
        </div>

        <div className="dashboard-card">
          <span>Tickets cerrados</span>
          <strong>
            {summary?.closedTickets ?? 0}
          </strong>
        </div>

        <div className="dashboard-card">
          <span>Conversaciones</span>
          <strong>
            {summary?.conversations ?? 0}
          </strong>
        </div>
      </div>

      <div className="dashboard-section">
        <div className="section-header">
          <h2>Tickets recientes</h2>
        </div>

        <div className="tickets-table-wrapper">
          <table className="tickets-table">
            <thead>
              <tr>
                <th>Ticket</th>
                <th>Usuario</th>
                <th>Email</th>
                <th>Estado</th>
                <th>Fecha</th>
              </tr>
            </thead>

            <tbody>
              {tickets.length === 0 ? (
                <tr>
                  <td colSpan={5}>
                    No hay tickets registrados.
                  </td>
                </tr>
              ) : (
                tickets
                  .slice(0, 10)
                  .map((ticket) => (
                    <tr key={ticket.id}>
                      <td>
                        {ticket.ticketNumber}
                      </td>

                      <td>
                        {ticket.name}
                      </td>

                      <td>
                        {ticket.email}
                      </td>

                      <td>
                        <span
                          className={`ticket-status ${ticket.status.toLowerCase()}`}
                        >
                          {ticket.status}
                        </span>
                      </td>

                      <td>
                        {new Date(
                          ticket.createdAt
                        ).toLocaleString()}
                      </td>
                    </tr>
                  ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;