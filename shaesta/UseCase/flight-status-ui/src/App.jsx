import React, { useState } from "react";

const statusColors = {
  OnTime: "#4caf50",
  Delayed: "#ff9800",
  Cancelled: "#f44336",
  Diverted: "#f44336",
  Unknown: "#9e9e9e",
};

export default function App() {
  const [flight, setFlight] = useState("");
  const [date, setDate] = useState("2026-05-23");
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  async function submit(e) {
    e.preventDefault();
    setError(null);
    setResult(null);
    try {
      const res = await fetch(
        `http://localhost:5000/flights/status?flightNumber=${encodeURIComponent(flight)}&date=${encodeURIComponent(date)}`,
      );
      if (!res.ok) throw new Error("API error");
      const data = await res.json();
      setResult(data);
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <div className="container">
      <h1>Flight Status</h1>
      <form onSubmit={submit} className="form">
        <input
          placeholder="Flight number"
          value={flight}
          onChange={(e) => setFlight(e.target.value)}
        />
        <input
          type="date"
          value={date}
          onChange={(e) => setDate(e.target.value)}
        />
        <button type="submit">Lookup</button>
      </form>
      {error && <div className="error">{error}</div>}
      {result && (
        <div className="card">
          <div
            className="status"
            style={{ background: statusColors[result.status] || "#ccc" }}
          >
            {result.status}
          </div>
          <div>Provider: {result.provider}</div>
          <div>
            Flight: {result.flightNumber} — {result.date}
          </div>
          {result.terminal && <div>Terminal: {result.terminal}</div>}
          {result.gate && <div>Gate: {result.gate}</div>}
          {result.delayReason && <div>Delay reason: {result.delayReason}</div>}
        </div>
      )}
    </div>
  );
}
