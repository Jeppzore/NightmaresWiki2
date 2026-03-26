export function LoadingBlock({ label = "Loading wiki data…" }: { label?: string }) {
  return <div className="panel panel--muted">{label}</div>;
}
