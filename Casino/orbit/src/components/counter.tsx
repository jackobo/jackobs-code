import React, { useState } from "react";

const Counter: React.FC = () => {
  const [times, setTimes] = useState(0);

  return (
    <div>
      <p>Times clicked {times}</p>
      <button onClick={() => setTimes(times + 1)}>Click</button>
    </div>
  );
};

export default Counter;
