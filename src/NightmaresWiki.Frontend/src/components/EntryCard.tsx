import { Link } from "react-router-dom";
import { resolveMediaUrl } from "../api/client";
import type { EntrySummary } from "../api/types";

function routeForEntry(entry: EntrySummary) {
  if (entry.type === "enemy") {
    return `/enemies/${entry.slug}`;
  }

  if (entry.type === "item") {
    return `/items/${entry.slug}`;
  }

  return "/npcs";
}

export function EntryCard({ entry }: { entry: EntrySummary }) {
  return (
    <article className="entry-card">
      {entry.image ? (
        <div className="entry-card__media">
          <img className="entry-card__image" src={resolveMediaUrl(entry.image)} alt={entry.title} />
        </div>
      ) : null}
      <div className="entry-card__body">
        <p className="entry-card__type">{entry.type}</p>
        <h3>
          <Link className="entry-card__title" to={routeForEntry(entry)}>
            {entry.title}
          </Link>
        </h3>
        <p>{entry.summary}</p>
        <div className="entry-card__taxonomy">
          {entry.taxonomy.map((item) => (
            <span key={item}>{item}</span>
          ))}
        </div>
        <Link className="entry-card__link" to={routeForEntry(entry)}>
          Read article
        </Link>
      </div>
    </article>
  );
}
