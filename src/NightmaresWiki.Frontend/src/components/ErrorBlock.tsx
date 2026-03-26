export function ErrorBlock({ label = "Something went wrong while loading this page." }: { label?: string }) {
  return <div className="panel panel--error">{label}</div>;
}
