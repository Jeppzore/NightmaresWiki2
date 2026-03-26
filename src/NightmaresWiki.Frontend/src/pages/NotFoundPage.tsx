import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <div className="page">
      <section className="panel panel--error">
        <h1>Page not found</h1>
        <p>The requested Nightmares article is missing or has not been imported yet.</p>
        <Link to="/">Return home</Link>
      </section>
    </div>
  );
}
