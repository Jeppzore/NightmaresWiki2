import { FormEvent, useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { searchEntries } from "../api/client";
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

export function SearchBox() {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<EntrySummary[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    if (query.trim().length < 2) {
      setResults([]);
      return;
    }

    const timeout = window.setTimeout(async () => {
      try {
        const payload = await searchEntries(query.trim());
        setResults(payload);
        setIsOpen(true);
      } catch {
        setResults([]);
      }
    }, 180);

    return () => window.clearTimeout(timeout);
  }, [query]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (results.length > 0) {
      navigate(routeForEntry(results[0]));
      setIsOpen(false);
    }
  }

  return (
    <div className="search">
      <form className="search__form" onSubmit={handleSubmit}>
        <input
          aria-label="Search entries"
          className="search__input"
          placeholder="Search enemies, items, and NPC..."
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          onFocus={() => setIsOpen(results.length > 0)}
        />
      </form>

      {isOpen && results.length > 0 ? (
        <div className="search__results" role="listbox">
          {results.map((result) => (
            <Link
              key={`${result.type}-${result.slug}`}
              to={routeForEntry(result)}
              className="search__result"
              onClick={() => setIsOpen(false)}
            >
              <span>{result.title}</span>
              <small>{result.type.toUpperCase()}</small>
            </Link>
          ))}
        </div>
      ) : null}
    </div>
  );
}
