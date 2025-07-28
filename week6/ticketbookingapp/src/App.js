import React, { useState } from 'react';
import { books } from './data';
import BookDetails from './components/BookDetails';
import BlogDetails from './components/BlogDetails';
import CourseDetails from './components/CourseDetails';

function App() {
  const [showBooks, setShowBooks] = useState(true);
  const [showBlogs, setShowBlogs] = useState(true);
  const [showCourses, setShowCourses] = useState(true);

  return (
    <div style={{ display: 'flex', gap: '40px', padding: '20px' }}>
      <div>
        {showCourses && <CourseDetails />}
        <button onClick={() => setShowCourses(!showCourses)}>
          {showCourses ? 'Hide' : 'Show'} Courses
        </button>
      </div>

      <div>
        {showBooks && <BookDetails books={books} />}
        <button onClick={() => setShowBooks(!showBooks)}>
          {showBooks ? 'Hide' : 'Show'} Books
        </button>
      </div>

      <div>
        {showBlogs && <BlogDetails />}
        <button onClick={() => setShowBlogs(!showBlogs)}>
          {showBlogs ? 'Hide' : 'Show'} Blogs
        </button>
      </div>
    </div>
  );
}

export default App;
